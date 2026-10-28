using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Displays the player's currency balance with a count-up animation.
    /// </summary>
    public sealed class WalletView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _balanceText;

        private Tween _countTween;
        private int _displayedBalance;

        public void SetIcon(Sprite sprite)
        {
            if (_icon != null && sprite != null)
                _icon.sprite = sprite;
        }

        public void SetBalance(int balance, bool animate)
        {
            if (!animate || _balanceText == null)
            {
                _displayedBalance = balance;
                if (_balanceText != null)
                    _balanceText.text = NumberFormatter.Format(balance);
                return;
            }

            _countTween?.Kill();
            int from = _displayedBalance;
            _countTween = DOVirtual.Int(from, balance, 0.3f, value =>
            {
                _displayedBalance = value;
                _balanceText.text = NumberFormatter.Format(value);
            })
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);

            _displayedBalance = balance;
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = ChildReference.FindChild<Image>(transform, "ui_image_wallet_icon");
            if (_balanceText == null)
                _balanceText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_wallet_balance_value");
        }

        private void OnDisable()
        {
            _countTween?.Kill();
            _countTween = null;
        }
    }
}
