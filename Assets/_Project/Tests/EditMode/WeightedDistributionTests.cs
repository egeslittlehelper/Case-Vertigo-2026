using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class WeightedDistributionTests
    {
        [Test]
        public void Resolve_10000Rolls_MatchesExpectedDistribution()
        {
            const int iterations = 10000;
            const float tolerance = 0.02f;

            var gold = new FakeRewardDefinition("gold");
            var silver = new FakeRewardDefinition("silver");
            var bomb = new FakeRewardDefinition("bomb");

            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(gold, baseAmount: 100, weight: 50),
                new FakeSliceDefinition(silver, baseAmount: 50, weight: 30),
                new FakeSliceDefinition(bomb, baseAmount: 0, weight: 20, isBomb: true));

            int totalWeight = 50 + 30 + 20;
            var counts = new int[3];

            var random = new SystemRandomProvider(seed: 42);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            for (int i = 0; i < iterations; i++)
            {
                SpinResult result = resolver.Resolve(wheel, zone: 1);
                counts[result.SliceIndex]++;
            }

            Assert.AreEqual(50f / totalWeight, counts[0] / (float)iterations, tolerance,
                "Gold frequency outside ±2%");
            Assert.AreEqual(30f / totalWeight, counts[1] / (float)iterations, tolerance,
                "Silver frequency outside ±2%");
            Assert.AreEqual(20f / totalWeight, counts[2] / (float)iterations, tolerance,
                "Bomb frequency outside ±2%");
        }
    }
}
