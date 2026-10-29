using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Shows "SUPER ZONE 30" and "SAFE ZONE 15" badges — updates and punches when values change.
    /// </summary>
    public sealed class ZoneInfoView : UIViewBase
    {
        [SerializeField] private TMP_Text _superValueText;
        [SerializeField] private TMP_Text _safeValueText;
        [SerializeField] private RectTransform _superPanel;
        [SerializeField] private RectTransform _safePanel;

        [SerializeField] private float _punchScale = 0.15f;
        [SerializeField] private float _punchDuration = 0.3f;

        private int _lastSuper;
        private int _lastSafe;

        private void Awake()
        {
            EnableAutoSizing(_superValueText);
            EnableAutoSizing(_safeValueText);
        }

        public void Set(int nextSafe, int nextSuper)
        {
            bool safeChanged = nextSafe != _lastSafe;
            bool superChanged = nextSuper != _lastSuper;

            _lastSafe = nextSafe;
            _lastSuper = nextSuper;

            if (_safeValueText != null)
                _safeValueText.text = nextSafe.ToString();

            if (_superValueText != null)
                _superValueText.text = nextSuper.ToString();

            if (safeChanged && _safePanel != null)
                PunchPanel(_safePanel);

            if (superChanged && _superPanel != null)
                PunchPanel(_superPanel);
        }

        private void PunchPanel(RectTransform panel)
        {
            panel.DOKill();
            panel.localScale = Vector3.one;
            panel.DOPunchScale(Vector3.one * _punchScale, _punchDuration, 1, 0.5f)
                .SetLink(panel.gameObject);
        }

        private static void EnableAutoSizing(TMP_Text text)
        {
            if (text == null) return;
            text.enableAutoSizing = true;
            text.fontSizeMin = 14f;
            text.fontSizeMax = 60f;
            text.alignment = TextAlignmentOptions.Center;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_superValueText == null)
                _superValueText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_zone_info_super_value");
            if (_safeValueText == null)
                _safeValueText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_zone_info_safe_value");
            if (_superPanel == null)
            {
                var t = ChildReference.FindChild<Transform>(transform, "ui_panel_zone_info_super");
                if (t != null) _superPanel = t as RectTransform;
            }
            if (_safePanel == null)
            {
                var t = ChildReference.FindChild<Transform>(transform, "ui_panel_zone_info_safe");
                if (t != null) _safePanel = t as RectTransform;
            }
        }
    }
}
