using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// One pooled flying icon — the fly animator moves and fades it via DOTween.
    /// </summary>
    public sealed class FlyIconView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public RectTransform RectTransform { get; private set; }

        public void SetIcon(Sprite sprite)
        {
            if (_icon != null)
            {
                _icon.sprite = sprite;
                _icon.enabled = sprite != null;
            }
        }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = ChildReference.FindChild<Image>(transform, "ui_image_fly_icon_value");
        }
    }
}
