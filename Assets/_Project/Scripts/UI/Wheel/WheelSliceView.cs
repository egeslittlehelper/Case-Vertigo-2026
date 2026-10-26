using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Displays one wheel slice — icon and amount text. Positioned by WheelView at build time.
    /// </summary>
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;

        public void Set(Sprite icon, string amountText)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.enabled = icon != null;
            }

            if (_amountText != null)
                _amountText.text = amountText;
        }

        public void Highlight()
        {
            // Placeholder for CP12 polish — punch scale or glow on the winning slice.
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = ChildReference.FindChild<Image>(transform, "ui_image_slice_icon_value");

            if (_amountText == null)
                _amountText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_slice_amount_value");
        }
    }
}
