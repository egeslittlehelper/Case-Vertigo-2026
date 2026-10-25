using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Determines zone type from configurable safe/super intervals.
    /// </summary>
    public sealed class ZoneRules : IZoneRules
    {
        private readonly int _safeInterval;
        private readonly int _superInterval;

        public ZoneRules(int safeInterval, int superInterval)
        {
            if (safeInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(safeInterval), "Must be > 0.");
            if (superInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(superInterval), "Must be > 0.");

            _safeInterval = safeInterval;
            _superInterval = superInterval;
        }

        public ZoneType GetZoneType(int zone)
        {
            if (zone <= 0)
                throw new ArgumentOutOfRangeException(nameof(zone), "Zones are 1-based.");

            if (zone % _superInterval == 0)
                return ZoneType.Super;

            if (zone % _safeInterval == 0)
                return ZoneType.Safe;

            return ZoneType.Normal;
        }

        public int GetNextZoneOfType(int fromZone, ZoneType type)
        {
            if (fromZone <= 0)
                throw new ArgumentOutOfRangeException(nameof(fromZone), "Zones are 1-based.");

            int interval = type switch
            {
                ZoneType.Safe => _safeInterval,
                ZoneType.Super => _superInterval,
                _ => throw new ArgumentException($"Cannot find next zone of type {type}.", nameof(type))
            };

            int remainder = fromZone % interval;
            return fromZone + (interval - remainder);
        }
    }
}
