using System;
using System.Collections.Generic;

namespace VertigoCase.Core
{
    /// <summary>
    /// Orchestrates one game run: zone progression, spin, rewards, and exit.
    /// </summary>
    public sealed class GameSession
    {
        private readonly IZoneRules _zoneRules;
        private readonly IWheelProvider _wheelProvider;
        private readonly SpinResolver _spinResolver;
        private readonly RewardInventory _inventory;
        private readonly GameStateMachine _stateMachine;

        private SpinResult? _pendingResult;

        public int CurrentZone { get; private set; }
        public ZoneType CurrentZoneType => _zoneRules.GetZoneType(CurrentZone);
        public GameState State => _stateMachine.Current;
        public IWheelDefinition CurrentWheel { get; private set; }
        public IReadOnlyList<RewardStack> CollectedRewards => _inventory.Items;

        public bool CanSpin => State == GameState.Idle;
        public bool CanExit => State == GameState.Idle && CurrentZoneType != ZoneType.Normal;

        public event Action<int, ZoneType> ZoneChanged;
        public event Action<SpinResult> SpinStarted;
        public event Action<SpinResult> RewardCollected;
        public event Action<SpinResult> BombHit;
        public event Action<IReadOnlyList<RewardStack>> CashedOut;
        public event Action RunReset;

        public GameSession(
            IZoneRules zoneRules,
            IWheelProvider wheelProvider,
            SpinResolver spinResolver,
            RewardInventory inventory)
        {
            _zoneRules = zoneRules ?? throw new ArgumentNullException(nameof(zoneRules));
            _wheelProvider = wheelProvider ?? throw new ArgumentNullException(nameof(wheelProvider));
            _spinResolver = spinResolver ?? throw new ArgumentNullException(nameof(spinResolver));
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _stateMachine = new GameStateMachine();

            CurrentZone = 1;
            RefreshWheel();
        }

        public SpinResult BeginSpin()
        {
            if (!CanSpin)
                throw new InvalidOperationException("Cannot spin in current state.");

            if (_pendingResult.HasValue)
                throw new InvalidOperationException("A spin is already pending.");

            SpinResult result = _spinResolver.Resolve(CurrentWheel, CurrentZone);
            _pendingResult = result;

            _stateMachine.TransitionTo(GameState.Spinning);
            SpinStarted?.Invoke(result);

            return result;
        }

        public void CompleteSpin()
        {
            if (State != GameState.Spinning)
                throw new InvalidOperationException("No spin in progress.");

            if (!_pendingResult.HasValue)
                throw new InvalidOperationException("No pending result.");

            SpinResult result = _pendingResult.Value;
            _pendingResult = null;

            if (result.IsBomb)
            {
                _stateMachine.TransitionTo(GameState.BombHit);
                BombHit?.Invoke(result);
            }
            else
            {
                _inventory.Add(result.Reward, result.Amount);
                CurrentZone++;
                RefreshWheel();

                _stateMachine.TransitionTo(GameState.Idle);
                RewardCollected?.Invoke(result);
                ZoneChanged?.Invoke(CurrentZone, CurrentZoneType);
            }
        }

        public void CashOut()
        {
            if (!CanExit)
                throw new InvalidOperationException(
                    "Can only cash out while idle in a Safe or Super zone.");

            var rewards = new List<RewardStack>(_inventory.Items);
            _stateMachine.TransitionTo(GameState.CashedOut);
            CashedOut?.Invoke(rewards);
        }

        public void GiveUp()
        {
            if (State != GameState.BombHit)
                throw new InvalidOperationException("GiveUp is only valid after a bomb hit.");

            ResetRun();
        }

        public void Restart()
        {
            if (State != GameState.CashedOut)
                throw new InvalidOperationException("Restart is only valid after cashing out.");

            ResetRun();
        }

        private void ResetRun()
        {
            _inventory.Clear();
            _pendingResult = null;
            CurrentZone = 1;
            RefreshWheel();

            _stateMachine.ForceReset();
            RunReset?.Invoke();
        }

        private void RefreshWheel()
        {
            CurrentWheel = _wheelProvider.GetWheelFor(CurrentZone, CurrentZoneType);
        }
    }
}
