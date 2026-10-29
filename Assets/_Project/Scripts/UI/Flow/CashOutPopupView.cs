using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Shows the list of collected rewards after cashing out, with a Play Again button.
    /// </summary>
    public sealed class CashOutPopupView : PopupViewBase
    {
        [Header("References")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Transform _rewardListContainer;
        [SerializeField] private RewardEntryView _entryPrefab;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _goBackButton;

        private readonly List<RewardEntryView> _activeEntries = new();
        private ObjectPool<RewardEntryView> _pool;

        public event Action ContinueClicked;
        public event Action GoBackClicked;

        public void Initialize()
        {
            _pool ??= new ObjectPool<RewardEntryView>(_entryPrefab, _rewardListContainer, 4);
            gameObject.SetActive(false);
        }

        public void Show(IReadOnlyList<RewardStack> rewards, RewardCatalogSO catalog)
        {
            ClearEntries();

            for (int i = 0; i < rewards.Count; i++)
            {
                RewardEntryView entry = _pool.Get();
                entry.EnableMaskable();
                catalog.TryGetIcon(rewards[i].Reward.Id, out Sprite icon);
                entry.Set(icon, rewards[i].Amount);
                _activeEntries.Add(entry);
            }

            base.Show();
        }

        public override void Hide(Action onHidden = null)
        {
            base.Hide(() =>
            {
                ClearEntries();
                onHidden?.Invoke();
            });
        }

        protected override string GetAnimTargetName() => "ui_anim_popup_cashout";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_titleText == null)
                _titleText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_cashout_title");
            if (_rewardListContainer == null)
                _rewardListContainer = ChildReference.FindChild<Transform>(transform, "ui_container_popup_cashout_rewards");
            if (_continueButton == null)
                _continueButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_cashout_continue");
            if (_goBackButton == null)
                _goBackButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_cashout_back");
#if UNITY_EDITOR
            if (_entryPrefab == null)
            {
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/_Project/Prefabs/UI/ui_reward_entry.prefab");
                if (prefab != null)
                    _entryPrefab = prefab.GetComponent<RewardEntryView>();
            }
#endif
        }

        private void OnEnable()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(HandleContinue);
            if (_goBackButton != null)
                _goBackButton.onClick.AddListener(HandleGoBack);
        }

        protected override void OnDisable()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(HandleContinue);
            if (_goBackButton != null)
                _goBackButton.onClick.RemoveListener(HandleGoBack);
            base.OnDisable();
        }

        private void HandleContinue() => ContinueClicked?.Invoke();
        private void HandleGoBack() => GoBackClicked?.Invoke();

        private void ClearEntries()
        {
            for (int i = 0; i < _activeEntries.Count; i++)
            {
                _activeEntries[i].ResetState();
                _pool.Release(_activeEntries[i]);
            }
            _activeEntries.Clear();
        }
    }
}
