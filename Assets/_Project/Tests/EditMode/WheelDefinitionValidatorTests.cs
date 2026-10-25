using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class WheelDefinitionValidatorTests
    {
        private FakeRewardDefinition _gold;
        private FakeRewardDefinition _bomb;

        [SetUp]
        public void SetUp()
        {
            _gold = new FakeRewardDefinition("gold");
            _bomb = new FakeRewardDefinition("bomb");
        }

        [Test]
        public void NormalWheel_OneBomb_NoErrors()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_gold, 50, 10),
                new FakeSliceDefinition(_bomb, 0, 5, isBomb: true));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void NormalWheel_NoBombs_ReturnsError()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_gold, 50, 10));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("exactly 1 bomb", errors[0]);
        }

        [Test]
        public void NormalWheel_TwoBombs_ReturnsError()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_bomb, 0, 5, isBomb: true),
                new FakeSliceDefinition(_bomb, 0, 5, isBomb: true));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("exactly 1 bomb", errors[0]);
        }

        [Test]
        public void SafeWheel_WithBomb_ReturnsError()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Safe,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_bomb, 0, 5, isBomb: true));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("no bombs", errors[0]);
        }

        [Test]
        public void SuperWheel_WithBomb_ReturnsError()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Super,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_bomb, 0, 5, isBomb: true));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("no bombs", errors[0]);
        }

        [Test]
        public void SafeWheel_NoBombs_NoErrors()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Safe,
                new FakeSliceDefinition(_gold, 100, 10),
                new FakeSliceDefinition(_gold, 200, 10));

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(0, errors.Count);
        }

        [Test]
        public void NullWheel_ReturnsError()
        {
            var errors = WheelDefinitionValidator.Validate(null);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("null", errors[0]);
        }

        [Test]
        public void EmptySlices_ReturnsError()
        {
            var wheel = new FakeWheelDefinition(ZoneType.Normal);

            var errors = WheelDefinitionValidator.Validate(wheel);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("no slices", errors[0]);
        }
    }
}
