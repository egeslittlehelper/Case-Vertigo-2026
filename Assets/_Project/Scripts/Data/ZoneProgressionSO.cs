using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Central config: zone intervals, reward scaling curve, and which wheels to use.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Zone Progression")]
    public sealed class ZoneProgressionSO : ScriptableObject
    {
        [Header("Zone Intervals")]
        [SerializeField] [Min(1)] private int _safeInterval = 5;
        [SerializeField] [Min(1)] private int _superInterval = 30;

        [Header("Reward Scaling")]
        [SerializeField] [Tooltip("X = zone, Y = multiplier. Zone 1 should be ~1.0.")]
        private AnimationCurve _rewardMultiplier = AnimationCurve.Linear(1, 1, 60, 10);

        [Header("Normal Wheels (by zone range)")]
        [SerializeField] private List<WheelTier> _normalTiers = new List<WheelTier>();

        [Header("Special Wheels")]
        [SerializeField] private WheelConfigSO _safeWheel;
        [SerializeField] private WheelConfigSO _superWheel;

        public int SafeInterval => _safeInterval;
        public int SuperInterval => _superInterval;
        public AnimationCurve RewardMultiplier => _rewardMultiplier;
        public IReadOnlyList<WheelTier> NormalTiers => _normalTiers;
        public WheelConfigSO SafeWheel => _safeWheel;
        public WheelConfigSO SuperWheel => _superWheel;

        public ZoneRules CreateZoneRules()
        {
            return new ZoneRules(_safeInterval, _superInterval);
        }
    }
}
