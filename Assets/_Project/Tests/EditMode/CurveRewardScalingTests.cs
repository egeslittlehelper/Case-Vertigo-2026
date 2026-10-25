using NUnit.Framework;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class CurveRewardScalingTests
    {
        [Test]
        public void Zone1_ReturnsBaseAmount()
        {
            var curve = AnimationCurve.Linear(1, 1, 60, 10);
            var scaling = new CurveRewardScaling(curve);

            int result = scaling.Scale(100, 1);

            Assert.AreEqual(100, result);
        }

        [Test]
        public void AmountsNeverDecrease_AsZoneIncreases()
        {
            var curve = AnimationCurve.Linear(1, 1, 60, 10);
            var scaling = new CurveRewardScaling(curve);

            int previous = 0;
            for (int zone = 1; zone <= 60; zone++)
            {
                int current = scaling.Scale(100, zone);
                Assert.GreaterOrEqual(current, previous,
                    $"Amount decreased at zone {zone}: {current} < {previous}");
                previous = current;
            }
        }

        [Test]
        public void HighZone_ReturnsScaledAmount()
        {
            var curve = AnimationCurve.Linear(1, 1, 60, 10);
            var scaling = new CurveRewardScaling(curve);

            int result = scaling.Scale(100, 60);

            Assert.AreEqual(1000, result);
        }

        [Test]
        public void Result_IsAlwaysAtLeastOne()
        {
            var curve = AnimationCurve.Constant(0, 100, 0.001f);
            var scaling = new CurveRewardScaling(curve);

            int result = scaling.Scale(1, 1);

            Assert.GreaterOrEqual(result, 1);
        }
    }
}
