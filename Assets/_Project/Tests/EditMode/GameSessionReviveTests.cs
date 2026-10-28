using System.Collections.Generic;
using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class GameSessionReviveTests
    {
        private FakeRewardDefinition _gold;
        private FakeRewardDefinition _currency;
        private FakeRewardDefinition _bombReward;
        private IWheelDefinition _normalWheel;
        private IWheelDefinition _safeWheel;
        private FakeWheelProvider _wheelProvider;

        [SetUp]
        public void SetUp()
        {
            _gold = new FakeRewardDefinition("gold");
            _currency = new FakeRewardDefinition("coin");
            _bombReward = new FakeRewardDefinition("bomb");

            _normalWheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_bombReward, baseAmount: 0, weight: 5, isBomb: true));

            _safeWheel = new FakeWheelDefinition(ZoneType.Safe,
                new FakeSliceDefinition(_currency, baseAmount: 50, weight: 10));

            _wheelProvider = new FakeWheelProvider();
            _wheelProvider.Register(ZoneType.Normal, _normalWheel);
            _wheelProvider.Register(ZoneType.Safe, _safeWheel);
            _wheelProvider.Register(ZoneType.Super, _safeWheel);
        }

        private GameSession CreateSession(IRandomProvider random, IWallet wallet,
            IReviveCostPolicy costPolicy = null, string currencyId = null)
        {
            var rules = new ZoneRules(5, 30);
            var scaling = new FakeRewardScaling();
            var resolver = new SpinResolver(random, scaling);
            var inventory = new RewardInventory();
            return new GameSession(rules, _wheelProvider, resolver, inventory,
                wallet, costPolicy ?? new EscalatingReviveCost(25, 2f), currencyId);
        }

        private GameSession SpinToBomb(IRandomProvider random, IWallet wallet,
            IReviveCostPolicy costPolicy = null, string currencyId = null)
        {
            var session = CreateSession(random, wallet, costPolicy, currencyId);
            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();
            return session;
        }

        [Test]
        public void CanRevive_WhenBombHit_AndCanAfford_ReturnsTrue()
        {
            var wallet = new InMemoryWallet(100);
            var session = SpinToBomb(new FakeRandomProvider(0, 10), wallet);

            Assert.AreEqual(GameState.BombHit, session.State);
            Assert.IsTrue(session.CanRevive);
        }

        [Test]
        public void CanRevive_WhenBombHit_CantAfford_ReturnsFalse()
        {
            var wallet = new InMemoryWallet(5);
            var session = SpinToBomb(new FakeRandomProvider(0, 10), wallet);

            Assert.AreEqual(GameState.BombHit, session.State);
            Assert.IsFalse(session.CanRevive);
        }

        [Test]
        public void CanRevive_WhenIdle_ReturnsFalse()
        {
            var wallet = new InMemoryWallet(100);
            var session = CreateSession(new FakeRandomProvider(0), wallet);

            Assert.AreEqual(GameState.Idle, session.State);
            Assert.IsFalse(session.CanRevive);
        }

        [Test]
        public void CanRevive_NoWallet_ReturnsFalse()
        {
            var rules = new ZoneRules(5, 30);
            var scaling = new FakeRewardScaling();
            var resolver = new SpinResolver(new FakeRandomProvider(0, 10), scaling);
            var inventory = new RewardInventory();
            var session = new GameSession(rules, _wheelProvider, resolver, inventory);

            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();

            Assert.AreEqual(GameState.BombHit, session.State);
            Assert.IsFalse(session.CanRevive);
        }

        [Test]
        public void TryRevive_SpendsFromWallet_AdvancesZone_ReturnsToIdle()
        {
            var wallet = new InMemoryWallet(100);
            var session = SpinToBomb(new FakeRandomProvider(0, 10), wallet);

            int zoneBefore = session.CurrentZone;
            bool revived = false;
            session.Revived += () => revived = true;

            bool result = session.TryRevive();

            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Idle, session.State);
            Assert.AreEqual(75, wallet.Balance);
            Assert.AreEqual(zoneBefore + 1, session.CurrentZone);
            Assert.IsTrue(revived);
        }

        [Test]
        public void TryRevive_KeepsInventory()
        {
            var wallet = new InMemoryWallet(100);
            var session = SpinToBomb(new FakeRandomProvider(0, 10), wallet);

            int countBefore = session.CollectedRewards.Count;
            session.TryRevive();

            Assert.AreEqual(countBefore, session.CollectedRewards.Count);
        }

        [Test]
        public void TryRevive_CostEscalates()
        {
            var rolls = new List<int> { 0, 10, 0, 10 };
            var wallet = new InMemoryWallet(200);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()), wallet,
                new EscalatingReviveCost(25, 2f));

            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();
            Assert.AreEqual(25, session.ReviveCost);
            session.TryRevive();
            Assert.AreEqual(175, wallet.Balance);

            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();
            Assert.AreEqual(50, session.ReviveCost);
            session.TryRevive();
            Assert.AreEqual(125, wallet.Balance);
        }

        [Test]
        public void TryRevive_CantAfford_ReturnsFalse_StateUnchanged()
        {
            var wallet = new InMemoryWallet(5);
            var session = SpinToBomb(new FakeRandomProvider(0, 10), wallet);

            bool result = session.TryRevive();

            Assert.IsFalse(result);
            Assert.AreEqual(GameState.BombHit, session.State);
            Assert.AreEqual(5, wallet.Balance);
        }

        [Test]
        public void GiveUp_ResetsReviveCount()
        {
            var rolls = new List<int> { 0, 10, 0, 10 };
            var wallet = new InMemoryWallet(500);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()), wallet,
                new EscalatingReviveCost(25, 2f));

            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();
            session.TryRevive();

            session.BeginSpin();
            session.CompleteSpin();
            session.BeginSpin();
            session.CompleteSpin();
            session.GiveUp();

            Assert.AreEqual(25, session.ReviveCost, "Revive count should reset after GiveUp.");
        }

        [Test]
        public void CashOut_DepositsCurrencyToWallet()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 4; i++) rolls.Add(0);

            var wallet = new InMemoryWallet(100);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()), wallet,
                currencyId: "gold");

            for (int i = 0; i < 4; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            session.CashOut();

            Assert.IsTrue(wallet.Balance > 100, "Wallet should increase after cashing out currency rewards.");
        }

        [Test]
        public void CashOut_NonCurrencyRewards_DoNotAffectWallet()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 4; i++) rolls.Add(0);

            var wallet = new InMemoryWallet(100);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()), wallet,
                currencyId: "diamond");

            for (int i = 0; i < 4; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            session.CashOut();

            Assert.AreEqual(100, wallet.Balance, "Non-currency rewards should not deposit to wallet.");
        }
    }
}
