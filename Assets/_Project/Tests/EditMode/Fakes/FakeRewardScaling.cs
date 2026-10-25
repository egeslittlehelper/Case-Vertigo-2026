using VertigoCase.Core;

namespace VertigoCase.Tests.Fakes
{
    /// <summary>
    /// Simple multiplier scaling for predictable test assertions.
    /// </summary>
    public sealed class FakeRewardScaling : IRewardScaling
    {
        public int Scale(int baseAmount, int zone)
        {
            return baseAmount * zone;
        }
    }
}
