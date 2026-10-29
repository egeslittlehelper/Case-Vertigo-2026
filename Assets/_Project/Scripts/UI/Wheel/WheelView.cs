using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Builds slices from a WheelConfigSO, applies the visual skin, and plays the DOTween spin.
    /// One prefab serves bronze, silver and golden — Bind() swaps everything.
    /// </summary>
    public sealed class WheelView : UIViewBase
    {
        [Header("References")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _subtitleText;
        [SerializeField] private Transform _slicesContainer;
        [SerializeField] private Button _spinButton;
        [SerializeField] private SliceViewFactory _sliceFactory;
        [SerializeField] private Transform _animTarget;

        [Header("Layout")]
        [SerializeField] [Range(50f, 600f)] private float _sliceContentRadius = 290f;

        [Header("Settings")]
        [SerializeField] private SpinAnimationSettingsSO _spinSettings;

        private Sequence _spinSequence;
        private Sequence _transitionSequence;
        private Tween _idlePulseTween;
        private Tween _tickTween;
        private WheelSliceView[] _slices;
        private int _sliceCount;
        private int _lastTickSlot;

        public event Action SpinClicked;

        public void Bind(IWheelDefinition wheel, WheelConfigSO visuals, int zone,
            IRewardScaling scaling, RewardCatalogSO catalog)
        {
            _sliceCount = wheel.Slices.Count;

            if (visuals != null)
            {
                if (_baseImage != null)
                    _baseImage.sprite = visuals.WheelBaseSprite;

                if (_indicatorImage != null)
                    _indicatorImage.sprite = visuals.IndicatorSprite;

                if (_titleText != null)
                {
                    _titleText.text = visuals.TitleText;
                    _titleText.color = visuals.TitleColor;
                }
            }

            NormalizeRotation();
            _slices = _sliceFactory.EnsureSlices(_sliceCount);

            float sliceAngle = WheelAngleCalculator.SliceAngle(_sliceCount);

            for (int i = 0; i < _sliceCount; i++)
            {
                ISliceDefinition slice = wheel.Slices[i];

                _slices[i].transform.localRotation =
                    Quaternion.Euler(0f, 0f, -i * sliceAngle);
                _slices[i].transform.localPosition = Vector3.zero;

                var contentContainer = _slices[i].transform.childCount > 0
                    ? _slices[i].transform.GetChild(0) as RectTransform
                    : null;
                if (contentContainer != null)
                    contentContainer.anchoredPosition = new Vector2(0f, _sliceContentRadius);

                Sprite icon = null;
                string amountText = "";

                if (slice.IsBomb)
                {
                    if (slice.Reward != null)
                        catalog.TryGetIcon(slice.Reward.Id, out icon);
                    amountText = "";
                }
                else if (slice.Reward != null)
                {
                    catalog.TryGetIcon(slice.Reward.Id, out icon);
                    int scaled = scaling != null ? scaling.Scale(slice.BaseAmount, zone) : slice.BaseAmount;
                    amountText = FormatAmount(scaled);
                }

                _slices[i].Set(icon, amountText);
            }
        }

        public void PlaySpin(int sliceIndex, Action onComplete)
        {
            SetInteractable(false);
            StopIdlePulse();
            ResetSliceVisuals();
            NormalizeRotation();

            float jitter = UnityEngine.Random.Range(-_spinSettings.LandingJitter, _spinSettings.LandingJitter);
            float jitteredTarget = WheelAngleCalculator.TargetRotation(
                sliceIndex, _sliceCount, _spinSettings.ExtraFullTurns, jitter);
            float exactCenter = WheelAngleCalculator.TargetRotation(
                sliceIndex, _sliceCount, _spinSettings.ExtraFullTurns, 0f);
            float overshoot = _spinSettings.SettleOvershoot;
            float sliceAngle = _sliceCount > 0 ? 360f / _sliceCount : 360f;

            float startZ = _animTarget.localEulerAngles.z;
            _lastTickSlot = Mathf.FloorToInt(startZ / sliceAngle);

            _spinSequence?.Kill();
            _spinSequence = DOTween.Sequence();

            if (_spinSettings.AnticipationAngle > 0f)
            {
                _spinSequence.Append(
                    _animTarget
                        .DOLocalRotate(new Vector3(0f, 0f, -_spinSettings.AnticipationAngle),
                            _spinSettings.AnticipationDuration)
                        .SetEase(Ease.OutQuad));
            }

            var mainSpin = _animTarget
                .DOLocalRotate(new Vector3(0f, 0f, jitteredTarget + overshoot),
                    _spinSettings.Duration, RotateMode.FastBeyond360)
                .SetEase(_spinSettings.SpinEase);

            if (_indicatorImage != null && _spinSettings.TickRotation > 0f)
            {
                mainSpin.OnUpdate(() =>
                {
                    float z = _animTarget.localEulerAngles.z;
                    int slot = Mathf.FloorToInt(z / sliceAngle);
                    if (slot != _lastTickSlot)
                    {
                        _lastTickSlot = slot;
                        PlayIndicatorTick();
                    }
                });
            }

            _spinSequence.Append(mainSpin);

            if (overshoot > 0f)
            {
                _spinSequence.Append(
                    _animTarget
                        .DOLocalRotate(new Vector3(0f, 0f, exactCenter), _spinSettings.SettleDuration)
                        .SetEase(Ease.InOutSine));
            }

            _spinSequence.AppendCallback(() => RevealResult(sliceIndex));

            if (_spinSettings.ResultHoldTime > 0f)
                _spinSequence.AppendInterval(_spinSettings.ResultHoldTime);

            _spinSequence.OnComplete(() => onComplete?.Invoke());
            _spinSequence.SetLink(gameObject);
        }

        public void PlayBombHit(Action onComplete)
        {
            var rt = _animTarget as RectTransform;
            if (rt == null)
            {
                onComplete?.Invoke();
                return;
            }

            rt.DOShakeAnchorPos(
                    _spinSettings.BombShakeDuration,
                    _spinSettings.BombShakeStrength,
                    _spinSettings.BombShakeVibrato)
                .SetLink(gameObject)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void StartIdlePulse()
        {
            if (_spinButton == null || _spinSettings == null) return;
            StopIdlePulse();

            var target = _spinButton.transform;
            _idlePulseTween = target
                .DOScale(1f + _spinSettings.IdlePulseStrength, _spinSettings.IdlePulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        public void StopIdlePulse()
        {
            _idlePulseTween?.Kill();
            _idlePulseTween = null;
            if (_spinButton != null)
                _spinButton.transform.localScale = Vector3.one;
        }

        public void TransitionTo(IWheelDefinition wheel, WheelConfigSO visuals, int zone,
            IRewardScaling scaling, RewardCatalogSO catalog)
        {
            if (_animTarget == null)
            {
                Bind(wheel, visuals, zone, scaling, catalog);
                return;
            }

            _transitionSequence?.Kill();
            _transitionSequence = DOTween.Sequence();

            _transitionSequence.Append(
                _animTarget.DOScale(0f, 0.2f).SetEase(Ease.InBack));

            _transitionSequence.AppendCallback(() =>
            {
                NormalizeRotation();
                Bind(wheel, visuals, zone, scaling, catalog);
            });

            _transitionSequence.Append(
                _animTarget.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

            _transitionSequence.SetLink(gameObject);
        }

        public RectTransform GetSliceTransform(int sliceIndex)
        {
            if (_slices == null || sliceIndex < 0 || sliceIndex >= _sliceCount)
                return null;

            var sliceRt = _slices[sliceIndex].GetComponent<RectTransform>();
            if (sliceRt.childCount > 0)
                return sliceRt.GetChild(0) as RectTransform;
            return sliceRt;
        }

        public void SetInteractable(bool interactable)
        {
            if (_spinButton != null)
                _spinButton.interactable = interactable;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_titleText == null)
                _titleText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_wheel_title_value");
            if (_baseImage == null)
                _baseImage = ChildReference.FindChild<Image>(transform, "ui_image_wheel_base_value");
            if (_indicatorImage == null)
                _indicatorImage = ChildReference.FindChild<Image>(transform, "ui_image_wheel_indicator_value");
            if (_subtitleText == null)
                _subtitleText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_wheel_subtitle_value");
            if (_slicesContainer == null)
                _slicesContainer = ChildReference.FindChild<Transform>(transform, "ui_container_wheel_slices");
            if (_spinButton == null)
                _spinButton = ChildReference.FindChild<Button>(transform, "ui_button_spin");
            if (_sliceFactory == null)
                _sliceFactory = GetComponentInChildren<SliceViewFactory>();
            if (_animTarget == null)
                _animTarget = ChildReference.FindChild<Transform>(transform, "ui_anim_wheel");
        }

        private void OnEnable()
        {
            if (_spinButton != null)
                _spinButton.onClick.AddListener(HandleSpinClicked);
        }

        private void OnDisable()
        {
            if (_spinButton != null)
                _spinButton.onClick.RemoveListener(HandleSpinClicked);

            _spinSequence?.Kill();
            _spinSequence = null;
            _transitionSequence?.Kill();
            _transitionSequence = null;
            _idlePulseTween?.Kill();
            _idlePulseTween = null;
            _tickTween?.Kill();
            _tickTween = null;
        }

        private void HandleSpinClicked()
        {
            SpinClicked?.Invoke();
        }

        private void PlayIndicatorTick()
        {
            _tickTween?.Kill();
            var indicatorRt = _indicatorImage.GetComponent<RectTransform>();
            indicatorRt.localRotation = Quaternion.identity;
            _tickTween = indicatorRt
                .DOPunchRotation(new Vector3(0f, 0f, -_spinSettings.TickRotation),
                    _spinSettings.TickDuration, 1, 0f)
                .SetLink(gameObject);
        }

        private void RevealResult(int sliceIndex)
        {
            if (_slices == null || _sliceCount <= 0) return;

            for (int i = 0; i < _sliceCount; i++)
            {
                if (i == sliceIndex)
                {
                    _slices[i].Highlight(
                        _spinSettings.HighlightPunchStrength,
                        _spinSettings.HighlightPunchDuration);
                }
                else
                {
                    _slices[i].SetDim(_spinSettings.DimAlpha, 0.2f);
                }
            }
        }

        private void ResetSliceVisuals()
        {
            if (_slices == null) return;
            for (int i = 0; i < _sliceCount; i++)
                _slices[i].ResetVisual();
        }

        private void NormalizeRotation()
        {
            if (_animTarget == null) return;
            Vector3 euler = _animTarget.localEulerAngles;
            euler.z = WheelAngleCalculator.NormalizeAngle(euler.z);
            _animTarget.localEulerAngles = euler;
        }

        private static string FormatAmount(int amount)
        {
            if (amount >= 1000)
                return $"x{amount / 1000f:0.#}K";
            return $"x{amount}";
        }
    }
}
