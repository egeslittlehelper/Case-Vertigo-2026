using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Transition-table state machine — prevents illegal moves like spin-while-spinning.
    /// </summary>
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; } = GameState.Idle;

        public event Action<GameState, GameState> Changed;

        public bool CanTransition(GameState to)
        {
            return (Current, to) switch
            {
                (GameState.Idle, GameState.Spinning) => true,
                (GameState.Idle, GameState.CashedOut) => true,
                (GameState.Spinning, GameState.Idle) => true,
                (GameState.Spinning, GameState.BombHit) => true,
                (GameState.BombHit, GameState.Idle) => true,
                (GameState.CashedOut, GameState.Idle) => true,
                _ => false
            };
        }

        public void TransitionTo(GameState to)
        {
            if (!CanTransition(to))
                throw new InvalidOperationException(
                    $"Illegal transition: {Current} → {to}.");

            GameState from = Current;
            Current = to;
            Changed?.Invoke(from, to);
        }

        public void ForceReset()
        {
            Current = GameState.Idle;
        }
    }
}
