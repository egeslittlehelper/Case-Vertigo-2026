using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// One row in the collected rewards panel — icon, amount text, and a punch-scale on update.
    /// </summary>
    public sealed class RewardEntryView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Transform _animTarget;

        [SerializeField] private LayoutElement _layoutElement;

        private Tween _countTween;
        private Tween _punchTween;
        private int _displayedAmount;

        public RectTransform RectTransform { get; private set; }

        public void EnableMaskable()
        {
            foreach (var g in GetComponentsInChildren<MaskableGraphic>(true))
                g.maskable = true;
        }

        public void Set(Sprite icon, int amount)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.enabled = icon != null;
            }

            _displayedAmount = amount;
            if (_amountText != null)
                _amountText.text = NumberFormatter.Format(amount);
        }

        public void AnimateTo(int targetAmount)
        {
            if (_amountText == null) return;

            _countTween?.Kill();
            _punchTween?.Kill();

            int from = _displayedAmount;
            _countTween = DOVirtual.Int(from, targetAmount, 0.25f, value =>
            {
                _displayedAmount = value;
                _amountText.text = NumberFormatter.Format(value);
            })
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);

            if (_animTarget != null)
            {
                _animTarget.localScale = Vector3.one;
                _punchTween = _animTarget
                    .DOPunchScale(Vector3.one * 0.15f, 0.3f, 6, 0.5f)
                    .SetLink(gameObject);
            }

            _displayedAmount = targetAmount;
        }

        public void ResetState()
        {
            _countTween?.Kill();
            _punchTween?.Kill();
            _displayedAmount = 0;

            if (_icon != null) _icon.sprite = null;
            if (_amountText != null) _amountText.text = "";
            if (_animTarget != null) _animTarget.localScale = Vector3.one;
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            FitToParentWidth();
        }

        private void FitToParentWidth()
        {
            if (RectTransform == null || _layoutElement == null) return;
            var parent = RectTransform.parent as RectTransform;
            if (parent == null) return;

            float panelWidth = parent.rect.width;
            float entryHeight = Mathf.Clamp(panelWidth * 0.55f, 40f, 100f);
            _layoutElement.preferredHeight = entryHeight;
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, entryHeight);
        }

        private void OnDisable()
        {
            _countTween?.Kill();
            _countTween = null;
            _punchTween?.Kill();
            _punchTween = null;
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = ChildReference.FindChild<Image>(transform, "ui_image_reward_icon_value");
            if (_amountText == null)
                _amountText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_reward_amount_value");
            if (_animTarget == null)
                _animTarget = ChildReference.FindChild<Transform>(transform, "ui_anim_reward_entry");
            if (_layoutElement == null)
                _layoutElement = GetComponent<LayoutElement>();
        }
    }
}
