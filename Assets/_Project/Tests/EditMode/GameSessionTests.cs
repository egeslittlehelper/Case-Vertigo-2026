using System;
using System.Collections.Generic;
using NUnit.Framework;
using VertigoCase.Core;
using VertigoCase.Tests.Fakes;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class GameSessionTests
    {
        private FakeRewardDefinition _gold;
        private FakeRewardDefinition _bombReward;
        private IWheelDefinition _normalWheel;
        private IWheelDefinition _safeWheel;
        private IWheelDefinition _superWheel;
        private FakeWheelProvider _wheelProvider;

        [SetUp]
        public void SetUp()
        {
            _gold = new FakeRewardDefinition("gold");
            _bombReward = new FakeRewardDefinition("bomb");

            _normalWheel = new FakeWheelDefinition(ZoneType.Normal,
                new FakeSliceDefinition(_gold, baseAmount: 100, weight: 10),
                new FakeSliceDefinition(_bombReward, baseAmount: 0, weight: 5, isBomb: true));

            _safeWheel = new FakeWheelDefinition(ZoneType.Safe,
                new FakeSliceDefinition(_gold, baseAmount: 200, weight: 10));

            _superWheel = new FakeWheelDefinition(ZoneType.Super,
                new FakeSliceDefinition(_gold, baseAmount: 500, weight: 10));

            _wheelProvider = new FakeWheelProvider();
            _wheelProvider.Register(ZoneType.Normal, _normalWheel);
            _wheelProvider.Register(ZoneType.Safe, _safeWheel);
            _wheelProvider.Register(ZoneType.Super, _superWheel);
        }

        private GameSession CreateSession(IRandomProvider random)
        {
            var rules = new ZoneRules(5, 30);
            var scaling = new FakeRewardScaling();
            var resolver = new SpinResolver(random, scaling);
            var inventory = new RewardInventory();
            return new GameSession(rules, _wheelProvider, resolver, inventory);
        }

        [Test]
        public void NewSession_StartsAtZone1_Idle_EmptyInventory_CannotExit()
        {
            var session = CreateSession(new FakeRandomProvider(0));

            Assert.AreEqual(1, session.CurrentZone);
            Assert.AreEqual(GameState.Idle, session.State);
            Assert.AreEqual(0, session.CollectedRewards.Count);
            Assert.IsFalse(session.CanExit);
            Assert.IsTrue(session.CanSpin);
        }

        [Test]
        public void RewardSpin_AddsToInventory_AdvancesZone_FiresEvents()
        {
            var session = CreateSession(new FakeRandomProvider(0));

            var firedEvents = new List<string>();
            session.RewardCollected += _ => firedEvents.Add("reward");
            session.ZoneChanged += (_, __) => firedEvents.Add("zone");

            SpinResult result = session.BeginSpin();
            Assert.AreEqual(GameState.Spinning, session.State);

            session.CompleteSpin();

            Assert.AreEqual(GameState.Idle, session.State);
            Assert.AreEqual(2, session.CurrentZone);
            Assert.AreEqual(1, session.CollectedRewards.Count);
            Assert.AreEqual(100, session.CollectedRewards[0].Amount);

            Assert.AreEqual(2, firedEvents.Count);
            Assert.AreEqual("reward", firedEvents[0]);
            Assert.AreEqual("zone", firedEvents[1]);
        }

        [Test]
        public void BombSpin_SetsBombHit_DoesNotClearInventory()
        {
            var session = CreateSession(new FakeRandomProvider(0, 10));

            session.BeginSpin();
            session.CompleteSpin();
            Assert.AreEqual(1, session.CollectedRewards.Count);

            bool bombFired = false;
            session.BombHit += _ => bombFired = true;

            session.BeginSpin();
            session.CompleteSpin();

            Assert.AreEqual(GameState.BombHit, session.State);
            Assert.IsTrue(bombFired);
            Assert.AreEqual(1, session.CollectedRewards.Count, "Inventory not yet cleared.");
        }

        [Test]
        public void GiveUp_ClearsInventory_ResetsToZone1()
        {
            var session = CreateSession(new FakeRandomProvider(0, 10));

            session.BeginSpin();
            session.CompleteSpin();

            session.BeginSpin();
            session.CompleteSpin();

            bool resetFired = false;
            session.RunReset += () => resetFired = true;

            session.GiveUp();

            Assert.AreEqual(GameState.Idle, session.State);
            Assert.AreEqual(1, session.CurrentZone);
            Assert.AreEqual(0, session.CollectedRewards.Count);
            Assert.IsTrue(resetFired);
        }

        [Test]
        public void BeginSpin_Twice_Throws()
        {
            var session = CreateSession(new FakeRandomProvider(0, 0));

            session.BeginSpin();
            Assert.Throws<InvalidOperationException>(() => session.BeginSpin());
        }

        [Test]
        public void CashOut_AtNormalZone_Throws()
        {
            var session = CreateSession(new FakeRandomProvider(0));

            Assert.AreEqual(ZoneType.Normal, session.CurrentZoneType);
            Assert.Throws<InvalidOperationException>(() => session.CashOut());
        }

        [Test]
        public void CashOut_AtSafeZone_Succeeds()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 4; i++) rolls.Add(0);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()));

            for (int i = 0; i < 4; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            Assert.AreEqual(5, session.CurrentZone);
            Assert.AreEqual(ZoneType.Safe, session.CurrentZoneType);

            IReadOnlyList<RewardStack> cashedRewards = null;
            session.CashedOut += r => cashedRewards = r;

            session.CashOut();

            Assert.AreEqual(GameState.CashedOut, session.State);
            Assert.IsNotNull(cashedRewards);
            Assert.IsTrue(cashedRewards.Count > 0);
        }

        [Test]
        public void CashOut_WhileSpinning_AtSafeZone_Throws()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 5; i++) rolls.Add(0);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()));

            for (int i = 0; i < 4; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            Assert.AreEqual(5, session.CurrentZone);
            session.BeginSpin();

            Assert.Throws<InvalidOperationException>(() => session.CashOut());
        }

        [Test]
        public void PlayToZone30_TypeIsSuper_WheelProviderAskedForSuper()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 29; i++) rolls.Add(0);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()));

            for (int i = 0; i < 29; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            Assert.AreEqual(30, session.CurrentZone);
            Assert.AreEqual(ZoneType.Super, session.CurrentZoneType);
            Assert.AreEqual(ZoneType.Super, _wheelProvider.LastRequestedType);
        }

        [Test]
        public void SafeWheel_NeverProducesBomb_Over1000Spins()
        {
            var random = new SystemRandomProvider(seed: 123);
            var rules = new ZoneRules(5, 30);
            var scaling = new FakeRewardScaling();
            var resolver = new SpinResolver(random, scaling);

            for (int i = 0; i < 1000; i++)
            {
                SpinResult result = resolver.Resolve(_safeWheel, zone: 5);
                Assert.IsFalse(result.IsBomb, $"Bomb appeared at iteration {i}.");
            }
        }

        [Test]
        public void Restart_AfterCashOut_ResetsRun()
        {
            var rolls = new List<int>();
            for (int i = 0; i < 4; i++) rolls.Add(0);
            var session = CreateSession(new FakeRandomProvider(rolls.ToArray()));

            for (int i = 0; i < 4; i++)
            {
                session.BeginSpin();
                session.CompleteSpin();
            }

            session.CashOut();

            bool resetFired = false;
            session.RunReset += () => resetFired = true;

            session.Restart();

            Assert.AreEqual(GameState.Idle, session.State);
            Assert.AreEqual(1, session.CurrentZone);
            Assert.AreEqual(0, session.CollectedRewards.Count);
            Assert.IsTrue(resetFired);
        }

        [Test]
        public void SpinStarted_FiresWithResult()
        {
            var session = CreateSession(new FakeRandomProvider(0));

            SpinResult? received = null;
            session.SpinStarted += r => received = r;

            session.BeginSpin();

            Assert.IsTrue(received.HasValue);
            Assert.AreEqual(0, received.Value.SliceIndex);
        }

        [Test]
        public void GiveUp_WhenNotBombHit_Throws()
        {
            var session = CreateSession(new FakeRandomProvider(0));
            Assert.Throws<InvalidOperationException>(() => session.GiveUp());
        }

        [Test]
        public void Restart_WhenNotCashedOut_Throws()
        {
            var session = CreateSession(new FakeRandomProvider(0));
            Assert.Throws<InvalidOperationException>(() => session.Restart());
        }
    }
}
