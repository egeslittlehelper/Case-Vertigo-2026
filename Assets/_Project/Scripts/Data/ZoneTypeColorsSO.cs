using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Maps each ZoneType to display colors — keeps hard-coded colors out of views.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Zone Type Colors")]
    public sealed class ZoneTypeColorsSO : ScriptableObject
    {
        [Header("Zone Number Text")]
        [SerializeField] private Color _normal = Color.white;
        [SerializeField] private Color _safe = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color _super = new Color(1f, 0.84f, 0f);

        [Header("Current Zone Highlight")]
        [SerializeField] private Color _highlightNormal = Color.white;
        [SerializeField] private Color _highlightSafe = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color _highlightSuper = new Color(1f, 0.84f, 0f);

        [Header("Current Zone Text (contrast with highlight)")]
        [SerializeField] private Color _currentTextNormal = Color.black;
        [SerializeField] private Color _currentTextSafe = Color.white;
        [SerializeField] private Color _currentTextSuper = Color.white;

        public Color GetColor(ZoneType type)
        {
            return type switch
            {
                ZoneType.Safe => _safe,
                ZoneType.Super => _super,
                _ => _normal
            };
        }

        public Color GetHighlightColor(ZoneType type)
        {
            return type switch
            {
                ZoneType.Safe => _highlightSafe,
                ZoneType.Super => _highlightSuper,
                _ => _highlightNormal
            };
        }

        public Color GetCurrentTextColor(ZoneType type)
        {
            return type switch
            {
                ZoneType.Safe => _currentTextSafe,
                ZoneType.Super => _currentTextSuper,
                _ => _currentTextNormal
            };
        }
    }
}
