using NUnit.Framework;
using VertigoCase.Core;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class ReviveCostTests
    {
        [Test]
        public void FirstRevive_ReturnsBaseCost()
        {
            var policy = new EscalatingReviveCost(baseCost: 25, multiplier: 2f);
            Assert.AreEqual(25, policy.GetCost(reviveCountThisRun: 0, zone: 1));
        }

        [Test]
        public void SecondRevive_ReturnsBaseTimesMultiplier()
        {
            var policy = new EscalatingReviveCost(baseCost: 25, multiplier: 2f);
            Assert.AreEqual(50, policy.GetCost(reviveCountThisRun: 1, zone: 1));
        }

        [Test]
        public void ThirdRevive_ReturnsBaseTimesMultiplierSquared()
        {
            var policy = new EscalatingReviveCost(baseCost: 25, multiplier: 2f);
            Assert.AreEqual(100, policy.GetCost(reviveCountThisRun: 2, zone: 1));
        }

        [Test]
        public void NegativeBaseCost_ClampedToOne()
        {
            var policy = new EscalatingReviveCost(baseCost: -5, multiplier: 2f);
            Assert.AreEqual(1, policy.GetCost(reviveCountThisRun: 0, zone: 1));
        }

        [Test]
        public void MultiplierBelowOne_ClampedToOne()
        {
            var policy = new EscalatingReviveCost(baseCost: 25, multiplier: 0.5f);
            Assert.AreEqual(25, policy.GetCost(reviveCountThisRun: 0, zone: 1));
            Assert.AreEqual(25, policy.GetCost(reviveCountThisRun: 1, zone: 1));
        }

        [Test]
        public void ZoneDoesNotAffectCost()
        {
            var policy = new EscalatingReviveCost(baseCost: 10, multiplier: 3f);
            int cost1 = policy.GetCost(reviveCountThisRun: 1, zone: 1);
            int cost2 = policy.GetCost(reviveCountThisRun: 1, zone: 30);
            Assert.AreEqual(cost1, cost2);
        }
    }
}
