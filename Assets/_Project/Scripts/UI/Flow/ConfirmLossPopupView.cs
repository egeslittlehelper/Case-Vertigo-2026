using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VertigoCase.UI
{
    /// <summary>
    /// Secondary confirmation before losing all rewards — "Lose the rewards" or "Go Back".
    /// </summary>
    public sealed class ConfirmLossPopupView : PopupViewBase
    {
        [Header("References")]
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _loseButton;
        [SerializeField] private Button _goBackButton;

        public event Action LoseClicked;
        public event Action GoBackClicked;

        protected override string GetAnimTargetName() => "ui_anim_popup_confirm";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_messageText == null)
                _messageText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_confirm_message");
            if (_loseButton == null)
                _loseButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_confirm_lose");
            if (_goBackButton == null)
                _goBackButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_confirm_back");
        }

        private void OnEnable()
        {
            if (_loseButton != null)
                _loseButton.onClick.AddListener(HandleLose);
            if (_goBackButton != null)
                _goBackButton.onClick.AddListener(HandleGoBack);
        }

        protected override void OnDisable()
        {
            if (_loseButton != null)
                _loseButton.onClick.RemoveListener(HandleLose);
            if (_goBackButton != null)
                _goBackButton.onClick.RemoveListener(HandleGoBack);
            base.OnDisable();
        }

        private void HandleLose() => LoseClicked?.Invoke();
        private void HandleGoBack() => GoBackClicked?.Invoke();
    }
}
