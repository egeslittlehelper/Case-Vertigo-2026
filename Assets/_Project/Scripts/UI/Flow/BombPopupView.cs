using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Popup shown when the player hits a bomb — offers Give Up (and Revive in CP11).
    /// </summary>
    public sealed class BombPopupView : PopupViewBase
    {
        [Header("References")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _lostText;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private TMP_Text _reviveCostText;

        public event Action GiveUpClicked;
        public event Action ReviveClicked;

        public void Show(int lostCount, bool canRevive, int reviveCost)
        {
            if (_lostText != null)
                _lostText.text = lostCount > 0
                    ? $"You lost {lostCount} reward(s)!"
                    : "You lost all rewards!";

            if (_reviveButton != null)
                _reviveButton.gameObject.SetActive(canRevive);

            if (_reviveCostText != null && canRevive)
                _reviveCostText.text = NumberFormatter.Format(reviveCost);

            base.Show();
        }

        protected override string GetAnimTargetName() => "ui_anim_popup_bomb";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_titleText == null)
                _titleText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_bomb_title");
            if (_lostText == null)
                _lostText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_bomb_lost_value");
            if (_giveUpButton == null)
                _giveUpButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_bomb_give_up");
            if (_reviveButton == null)
                _reviveButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_bomb_revive");
            if (_reviveCostText == null)
                _reviveCostText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_bomb_revive_cost_value");
        }

        private void OnEnable()
        {
            if (_giveUpButton != null)
                _giveUpButton.onClick.AddListener(HandleGiveUp);
            if (_reviveButton != null)
                _reviveButton.onClick.AddListener(HandleRevive);
        }

        protected override void OnDisable()
        {
            if (_giveUpButton != null)
                _giveUpButton.onClick.RemoveListener(HandleGiveUp);
            if (_reviveButton != null)
                _reviveButton.onClick.RemoveListener(HandleRevive);
            base.OnDisable();
        }

        private void HandleGiveUp() => GiveUpClicked?.Invoke();
        private void HandleRevive() => ReviveClicked?.Invoke();
    }
}
