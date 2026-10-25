using System;
using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class RewardInventoryTests
    {
        private RewardInventory _inventory;
        private FakeRewardDefinition _gold;
        private FakeRewardDefinition _silver;

        [SetUp]
        public void SetUp()
        {
            _inventory = new RewardInventory();
            _gold = new FakeRewardDefinition("gold");
            _silver = new FakeRewardDefinition("silver");
        }

        [Test]
        public void Add_NewReward_CreatesStack()
        {
            _inventory.Add(_gold, 100);

            Assert.AreEqual(1, _inventory.Items.Count);
            Assert.AreEqual("gold", _inventory.Items[0].Reward.Id);
            Assert.AreEqual(100, _inventory.Items[0].Amount);
        }

        [Test]
        public void Add_SameReward_MergesStacks()
        {
            _inventory.Add(_gold, 100);
            _inventory.Add(_gold, 50);

            Assert.AreEqual(1, _inventory.Items.Count);
            Assert.AreEqual(150, _inventory.Items[0].Amount);
        }

        [Test]
        public void Add_DifferentRewards_SeparateStacks()
        {
            _inventory.Add(_gold, 100);
            _inventory.Add(_silver, 50);

            Assert.AreEqual(2, _inventory.Items.Count);
        }

        [Test]
        public void Clear_EmptiesItems()
        {
            _inventory.Add(_gold, 100);
            _inventory.Clear();

            Assert.AreEqual(0, _inventory.Items.Count);
        }

        [Test]
        public void Clear_RaisesCleared()
        {
            bool fired = false;
            _inventory.Cleared += () => fired = true;

            _inventory.Add(_gold, 100);
            _inventory.Clear();

            Assert.IsTrue(fired);
        }

        [Test]
        public void Add_RaisesItemChanged_WithNewTotal()
        {
            RewardStack received = default;
            _inventory.ItemChanged += stack => received = stack;

            _inventory.Add(_gold, 100);
            Assert.AreEqual(100, received.Amount);

            _inventory.Add(_gold, 50);
            Assert.AreEqual(150, received.Amount);
        }

        [Test]
        public void Add_NullReward_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _inventory.Add(null, 100));
        }

        [Test]
        public void Add_ZeroAmount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _inventory.Add(_gold, 0));
        }

        [Test]
        public void Add_NegativeAmount_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _inventory.Add(_gold, -5));
        }
    }
}
