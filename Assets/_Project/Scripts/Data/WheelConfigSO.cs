using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// One wheel variant — slices editable in the inspector, validated by Core rules.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Wheel Config")]
    public sealed class WheelConfigSO : ScriptableObject, IWheelDefinition
    {
        [Header("Game")]
        [SerializeField] private ZoneType _zoneType;
        [SerializeField] private List<WheelSliceData> _slices = new List<WheelSliceData>();

        [Header("Visuals")]
        [SerializeField] private Sprite _wheelBaseSprite;
        [SerializeField] private Sprite _indicatorSprite;
        [SerializeField] private string _titleText = "SPIN";
        [SerializeField] private Color _titleColor = Color.white;

        public IReadOnlyList<ISliceDefinition> Slices => _slices;
        public ZoneType ZoneType => _zoneType;

        public Sprite WheelBaseSprite => _wheelBaseSprite;
        public Sprite IndicatorSprite => _indicatorSprite;
        public string TitleText => _titleText;
        public Color TitleColor => _titleColor;

        private void OnValidate()
        {
            var errors = WheelDefinitionValidator.Validate(this);
            for (int i = 0; i < errors.Count; i++)
                Debug.LogError($"[{name}] {errors[i]}", this);

            for (int i = 0; i < _slices.Count; i++)
            {
                if (!_slices[i].IsBomb && _slices[i].Reward == null)
                    Debug.LogError($"[{name}] Slice {i} has no reward assigned.", this);
            }
        }
    }
}
