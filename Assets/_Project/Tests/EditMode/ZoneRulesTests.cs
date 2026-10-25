using System;
using NUnit.Framework;
using VertigoCase.Core;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class ZoneRulesTests
    {
        private ZoneRules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = new ZoneRules(safeInterval: 5, superInterval: 30);
        }

        [TestCase(1, ZoneType.Normal)]
        [TestCase(4, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(10, ZoneType.Safe)]
        [TestCase(29, ZoneType.Normal)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(60, ZoneType.Super)]
        [TestCase(35, ZoneType.Safe)]
        public void GetZoneType_ReturnsCorrectType(int zone, ZoneType expected)
        {
            Assert.AreEqual(expected, _rules.GetZoneType(zone));
        }

        [Test]
        public void GetZoneType_SuperTakesPriorityOverSafe()
        {
            Assert.AreEqual(ZoneType.Super, _rules.GetZoneType(30));
            Assert.AreEqual(ZoneType.Super, _rules.GetZoneType(60));
        }

        [Test]
        public void GetZoneType_ZoneZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _rules.GetZoneType(0));
        }

        [Test]
        public void GetZoneType_NegativeZone_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _rules.GetZoneType(-1));
        }

        [Test]
        public void GetNextZoneOfType_Safe_FromZone11_Returns15()
        {
            Assert.AreEqual(15, _rules.GetNextZoneOfType(11, ZoneType.Safe));
        }

        [Test]
        public void GetNextZoneOfType_Super_FromZone11_Returns30()
        {
            Assert.AreEqual(30, _rules.GetNextZoneOfType(11, ZoneType.Super));
        }

        [Test]
        public void GetNextZoneOfType_Super_FromZone30_Returns60()
        {
            Assert.AreEqual(60, _rules.GetNextZoneOfType(30, ZoneType.Super));
        }

        [Test]
        public void GetNextZoneOfType_Safe_FromZone5_Returns10()
        {
            Assert.AreEqual(10, _rules.GetNextZoneOfType(5, ZoneType.Safe));
        }

        [Test]
        public void GetNextZoneOfType_Normal_Throws()
        {
            Assert.Throws<ArgumentException>(() => _rules.GetNextZoneOfType(1, ZoneType.Normal));
        }

        [Test]
        public void Constructor_ZeroSafeInterval_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ZoneRules(0, 30));
        }

        [Test]
        public void Constructor_ZeroSuperInterval_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ZoneRules(5, 0));
        }
    }
}
