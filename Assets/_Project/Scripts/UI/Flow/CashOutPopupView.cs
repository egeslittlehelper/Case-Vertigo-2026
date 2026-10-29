using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Confirmation popup before cashing out — rewards are already visible in the side panel.
    /// </summary>
    public sealed class CashOutPopupView : PopupViewBase
    {
        [Header("References")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _goBackButton;

        public event Action ContinueClicked;
        public event Action GoBackClicked;

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            base.Show();
        }

        protected override string GetAnimTargetName() => "ui_anim_popup_cashout";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_titleText == null)
                _titleText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_popup_cashout_title");
            if (_continueButton == null)
                _continueButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_cashout_continue");
            if (_goBackButton == null)
                _goBackButton = ChildReference.FindChild<Button>(transform, "ui_button_popup_cashout_back");
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
    }
}
