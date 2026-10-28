using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Each revive in a run costs more: baseCost * multiplier^reviveCount.
    /// </summary>
    public sealed class EscalatingReviveCost : IReviveCostPolicy
    {
        private readonly int _baseCost;
        private readonly float _multiplier;

        public EscalatingReviveCost(int baseCost, float multiplier)
        {
            _baseCost = Math.Max(1, baseCost);
            _multiplier = Math.Max(1f, multiplier);
        }

        public int GetCost(int reviveCountThisRun, int zone)
        {
            if (reviveCountThisRun <= 0) return _baseCost;
            return (int)(_baseCost * Math.Pow(_multiplier, reviveCountThisRun));
        }
    }
}
