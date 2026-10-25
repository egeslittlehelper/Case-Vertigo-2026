using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Scales rewards using an AnimationCurve from ZoneProgressionSO, rounding to clean display values.
    /// </summary>
    public sealed class CurveRewardScaling : IRewardScaling
    {
        private readonly AnimationCurve _curve;

        public CurveRewardScaling(AnimationCurve curve)
        {
            _curve = curve;
        }

        public int Scale(int baseAmount, int zone)
        {
            float multiplier = _curve.Evaluate(zone);
            int raw = Mathf.RoundToInt(baseAmount * multiplier);
            return RoundToNice(Mathf.Max(raw, 1));
        }

        private static int RoundToNice(int value)
        {
            if (value <= 10) return value;
            if (value <= 100) return ((value + 2) / 5) * 5;
            if (value <= 1000) return ((value + 5) / 10) * 10;
            return ((value + 50) / 100) * 100;
        }
    }
}
