using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Picks a slice by weighted random and scales the reward for the current zone.
    /// </summary>
    public sealed class SpinResolver
    {
        private readonly IRandomProvider _random;
        private readonly IRewardScaling _scaling;

        public SpinResolver(IRandomProvider random, IRewardScaling scaling)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _scaling = scaling ?? throw new ArgumentNullException(nameof(scaling));
        }

        public SpinResult Resolve(IWheelDefinition wheel, int zone)
        {
            if (wheel == null) throw new ArgumentNullException(nameof(wheel));
            if (zone <= 0) throw new ArgumentOutOfRangeException(nameof(zone), "Zones are 1-based.");

            var slices = wheel.Slices;
            int totalWeight = 0;
            for (int i = 0; i < slices.Count; i++)
                totalWeight += slices[i].Weight;

            if (totalWeight <= 0)
                throw new InvalidOperationException("Total weight must be > 0.");

            int roll = _random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < slices.Count; i++)
            {
                cumulative += slices[i].Weight;
                if (roll < cumulative)
                {
                    var slice = slices[i];
                    int amount = slice.IsBomb ? 0 : _scaling.Scale(slice.BaseAmount, zone);
                    return new SpinResult(i, slice.IsBomb, slice.Reward, amount);
                }
            }

            // Should be unreachable if weights are positive
            throw new InvalidOperationException("Failed to resolve a slice.");
        }
    }
}
