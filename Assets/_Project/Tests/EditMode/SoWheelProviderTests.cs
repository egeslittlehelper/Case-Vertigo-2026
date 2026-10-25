using NUnit.Framework;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class SoWheelProviderTests
    {
        private ZoneProgressionSO _progression;
        private WheelConfigSO _tier1;
        private WheelConfigSO _tier2;
        private WheelConfigSO _silver;
        private WheelConfigSO _golden;

        [SetUp]
        public void SetUp()
        {
            _tier1 = ScriptableObject.CreateInstance<WheelConfigSO>();
            _tier2 = ScriptableObject.CreateInstance<WheelConfigSO>();
            _silver = ScriptableObject.CreateInstance<WheelConfigSO>();
            _golden = ScriptableObject.CreateInstance<WheelConfigSO>();

            _progression = ScriptableObject.CreateInstance<ZoneProgressionSO>();

            SetPrivateField(_progression, "_safeInterval", 5);
            SetPrivateField(_progression, "_superInterval", 30);
            SetPrivateField(_progression, "_safeWheel", _silver);
            SetPrivateField(_progression, "_superWheel", _golden);

            var tiers = new System.Collections.Generic.List<WheelTier>();

            var t1 = new WheelTier();
            SetPrivateField(t1, "_minZone", 1);
            SetPrivateField(t1, "_wheel", _tier1);
            tiers.Add(t1);

            var t2 = new WheelTier();
            SetPrivateField(t2, "_minZone", 11);
            SetPrivateField(t2, "_wheel", _tier2);
            tiers.Add(t2);

            SetPrivateField(_progression, "_normalTiers", tiers);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_progression);
            Object.DestroyImmediate(_tier1);
            Object.DestroyImmediate(_tier2);
            Object.DestroyImmediate(_silver);
            Object.DestroyImmediate(_golden);
        }

        [Test]
        public void SafeZone_ReturnsSilverWheel()
        {
            var provider = new SoWheelProvider(_progression);
            var wheel = provider.GetWheelFor(5, ZoneType.Safe);

            Assert.AreEqual(_silver, wheel);
        }

        [Test]
        public void SuperZone_ReturnsGoldenWheel()
        {
            var provider = new SoWheelProvider(_progression);
            var wheel = provider.GetWheelFor(30, ZoneType.Super);

            Assert.AreEqual(_golden, wheel);
        }

        [Test]
        public void Zone1_ReturnsTier1()
        {
            var provider = new SoWheelProvider(_progression);
            var wheel = provider.GetWheelFor(1, ZoneType.Normal);

            Assert.AreEqual(_tier1, wheel);
        }

        [Test]
        public void Zone12_ReturnsTier2()
        {
            var provider = new SoWheelProvider(_progression);
            var wheel = provider.GetWheelFor(12, ZoneType.Normal);

            Assert.AreEqual(_tier2, wheel);
        }

        [Test]
        public void Zone10_StillReturnsTier1()
        {
            var provider = new SoWheelProvider(_progression);
            var wheel = provider.GetWheelFor(10, ZoneType.Normal);

            Assert.AreEqual(_tier1, wheel);
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var field = type.GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, $"Field '{fieldName}' not found on {type.Name}");
            field.SetValue(obj, value);
        }
    }
}
