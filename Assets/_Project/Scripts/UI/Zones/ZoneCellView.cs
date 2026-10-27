using TMPro;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// One cell in the zone strip — shows a zone number colored by its type, or by contrast when current.
    /// </summary>
    public sealed class ZoneCellView : MonoBehaviour
    {
        private const int CacheSize = 200;
        private static readonly string[] ZoneStringCache = BuildCache();

        [SerializeField] private TMP_Text _numberText;

        private int _zone;

        public int Zone => _zone;

        public void Set(int zone, ZoneType type, ZoneTypeColorsSO colors, bool isCurrent = false)
        {
            _zone = zone;

            if (_numberText != null)
            {
                _numberText.text = ZoneToString(zone);
                _numberText.color = colors != null
                    ? (isCurrent ? colors.GetCurrentTextColor(type) : colors.GetColor(type))
                    : Color.white;
            }
        }

        private static string[] BuildCache()
        {
            var cache = new string[CacheSize];
            for (int i = 0; i < CacheSize; i++)
                cache[i] = i.ToString();
            return cache;
        }

        private static string ZoneToString(int zone)
        {
            return zone >= 0 && zone < CacheSize ? ZoneStringCache[zone] : zone.ToString();
        }

        public void EnableMaskable()
        {
            if (_numberText != null)
                _numberText.maskable = true;
        }

        private void OnValidate()
        {
            if (_numberText == null)
                _numberText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_zone_number_value");
        }
    }
}
