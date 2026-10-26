using NUnit.Framework;
using VertigoCase.UI;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class WheelAngleCalculatorTests
    {
        private const float Tolerance = 0.001f;

        [Test]
        public void SliceAngle_EightSlices_Returns45()
        {
            Assert.AreEqual(45f, WheelAngleCalculator.SliceAngle(8), Tolerance);
        }

        [Test]
        public void SliceAngle_SixSlices_Returns60()
        {
            Assert.AreEqual(60f, WheelAngleCalculator.SliceAngle(6), Tolerance);
        }

        [Test]
        public void SliceCenterAngle_Index2_Of8_Returns90()
        {
            Assert.AreEqual(90f, WheelAngleCalculator.SliceCenterAngle(2, 8), Tolerance);
        }

        [Test]
        public void SliceCenterAngle_Index0_Returns0()
        {
            Assert.AreEqual(0f, WheelAngleCalculator.SliceCenterAngle(0, 8), Tolerance);
        }

        [Test]
        public void SliceCenterAngle_SevenSlices_FloatTolerance()
        {
            float expected = 2f * 360f / 7f;
            Assert.AreEqual(expected, WheelAngleCalculator.SliceCenterAngle(2, 7), Tolerance);
        }

        [TestCase(8)]
        [TestCase(6)]
        [TestCase(7)]
        public void TargetRotation_NoJitter_Mod360_EqualsSliceCenter(int count)
        {
            const int fullTurns = 5;

            for (int i = 0; i < count; i++)
            {
                float target = WheelAngleCalculator.TargetRotation(i, count, fullTurns, 0f);
                float expected = i * 360f / count;
                float actual = target % 360f;

                Assert.AreEqual(expected, actual, Tolerance,
                    $"Slice {i} of {count}: target mod 360 should be {expected}, was {actual}");
            }
        }

        [Test]
        public void TargetRotation_IncludesFullTurns()
        {
            float target = WheelAngleCalculator.TargetRotation(0, 8, 5, 0f);
            Assert.GreaterOrEqual(target, 5 * 360f);
        }

        [Test]
        public void TargetRotation_JitterOffsetApplied()
        {
            float withoutJitter = WheelAngleCalculator.TargetRotation(3, 8, 5, 0f);
            float withJitter = WheelAngleCalculator.TargetRotation(3, 8, 5, 0.2f);
            float sliceAngle = WheelAngleCalculator.SliceAngle(8);

            float diff = withJitter - withoutJitter;
            Assert.AreEqual(0.2f * sliceAngle, diff, Tolerance);
        }

        [Test]
        public void NormalizeAngle_PositiveLargeValue()
        {
            float result = WheelAngleCalculator.NormalizeAngle(730f);
            Assert.AreEqual(10f, result, Tolerance);
        }

        [Test]
        public void NormalizeAngle_NegativeValue()
        {
            float result = WheelAngleCalculator.NormalizeAngle(-90f);
            Assert.AreEqual(270f, result, Tolerance);
        }

        [Test]
        public void NormalizeAngle_Zero_ReturnsZero()
        {
            Assert.AreEqual(0f, WheelAngleCalculator.NormalizeAngle(0f), Tolerance);
        }

        [Test]
        public void NormalizeAngle_360_ReturnsZero()
        {
            Assert.AreEqual(0f, WheelAngleCalculator.NormalizeAngle(360f), Tolerance);
        }
    }
}
