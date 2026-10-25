using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Immutable value pairing a reward with its collected amount.
    /// </summary>
    public readonly struct RewardStack : IEquatable<RewardStack>
    {
        public IRewardDefinition Reward { get; }
        public int Amount { get; }

        public RewardStack(IRewardDefinition reward, int amount)
        {
            Reward = reward ?? throw new ArgumentNullException(nameof(reward));
            Amount = amount;
        }

        public RewardStack Add(int extra)
        {
            return new RewardStack(Reward, Amount + extra);
        }

        public bool Equals(RewardStack other)
        {
            return Amount == other.Amount
                && string.Equals(Reward?.Id, other.Reward?.Id, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) => obj is RewardStack other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Reward?.Id?.GetHashCode() ?? 0) * 397) ^ Amount;
            }
        }

        public override string ToString() => $"{Reward?.Id} x{Amount}";
    }
}
