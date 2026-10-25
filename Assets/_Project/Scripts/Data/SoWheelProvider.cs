using System;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Picks the right wheel by zone type and tier — wraps ZoneProgressionSO for the domain.
    /// </summary>
    public sealed class SoWheelProvider : IWheelProvider
    {
        private readonly ZoneProgressionSO _progression;

        public SoWheelProvider(ZoneProgressionSO progression)
        {
            _progression = progression ?? throw new ArgumentNullException(nameof(progression));
        }

        public IWheelDefinition GetWheelFor(int zone, ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Safe:
                    return _progression.SafeWheel;
                case ZoneType.Super:
                    return _progression.SuperWheel;
                default:
                    return GetNormalWheelForZone(zone);
            }
        }

        private IWheelDefinition GetNormalWheelForZone(int zone)
        {
            var tiers = _progression.NormalTiers;
            WheelConfigSO best = null;
            int bestMinZone = 0;

            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i].MinZone <= zone && tiers[i].Wheel != null
                    && tiers[i].MinZone > bestMinZone)
                {
                    best = tiers[i].Wheel;
                    bestMinZone = tiers[i].MinZone;
                }
            }

            if (best == null)
                throw new InvalidOperationException(
                    $"No normal wheel tier configured for zone {zone}.");

            return best;
        }
    }
}
