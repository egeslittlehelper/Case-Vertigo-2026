using System;
using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class SpinResolverTests
    {
        private FakeRewardDefinition _gold;
        private FakeRewardDefinition _silver;
        private FakeRewardDefinition _bomb;

        [SetUp]
        public void SetUp()
        {
            _gold = new FakeRewardDefinition("gold");
            _silver = new FakeRewardDefinition("silver");
            _bomb = new FakeRewardDefinition("bomb");
        }

        [Test]
        public void Resolve_PicksFirstSlice_WhenRollIsZero()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_silver, baseAmount: 50, weight: 10),
                new FakeSliceDefinition(_bomb, baseAmount: 0, weight: 5, isBomb: true));

            var random = new FakeRandomProvider(0);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            SpinResult result = resolver.Resolve(wheel, zone: 1);

            Assert.AreEqual(0, result.SliceIndex);
            Assert.IsFalse(result.IsBomb);
            Assert.AreEqual("gold", result.Reward.Id);
        }

        [Test]
        public void Resolve_PicksSecondSlice_WhenRollCrossesFirstWeight()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_silver, baseAmount: 50, weight: 10),
                new FakeSliceDefinition(_bomb, baseAmount: 0, weight: 5, isBomb: true));

            var random = new FakeRandomProvider(10);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            SpinResult result = resolver.Resolve(wheel, zone: 1);

            Assert.AreEqual(1, result.SliceIndex);
            Assert.AreEqual("silver", result.Reward.Id);
        }

        [Test]
        public void Resolve_PicksBomb_WhenRollHitsLastSlice()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_silver, baseAmount: 50, weight: 10),
                new FakeSliceDefinition(_bomb, baseAmount: 0, weight: 5, isBomb: true));

            var random = new FakeRandomProvider(20);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            SpinResult result = resolver.Resolve(wheel, zone: 1);

            Assert.AreEqual(2, result.SliceIndex);
            Assert.IsTrue(result.IsBomb);
            Assert.AreEqual(0, result.Amount);
        }

        [Test]
        public void Resolve_ScalesAmount_ByZone()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Safe,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10));

            var random = new FakeRandomProvider(0);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            SpinResult result = resolver.Resolve(wheel, zone: 5);

            Assert.AreEqual(500, result.Amount);
        }

        [Test]
        public void Resolve_ZeroTotalWeight_Throws()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 0));

            var random = new FakeRandomProvider(0);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(wheel, zone: 1));
        }

        [Test]
        public void Resolve_ZoneZero_Throws()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_bomb, baseAmount: 0, weight: 5, isBomb: true));

            var random = new FakeRandomProvider(0);
            var resolver = new SpinResolver(random, new FakeRewardScaling());

            Assert.Throws<ArgumentOutOfRangeException>(() => resolver.Resolve(wheel, zone: 0));
        }
    }
}
