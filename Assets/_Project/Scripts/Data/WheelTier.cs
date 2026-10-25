using System;
using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// Maps a zone range to a normal wheel config — higher tiers have better rewards.
    /// </summary>
    [Serializable]
    public sealed class WheelTier
    {
        [SerializeField] [Min(1)] [Tooltip("First zone that uses this wheel.")]
        private int _minZone = 1;

        [SerializeField] private WheelConfigSO _wheel;

        public int MinZone => _minZone;
        public WheelConfigSO Wheel => _wheel;
    }
}
