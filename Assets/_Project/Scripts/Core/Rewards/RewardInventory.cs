using System;
using System.Collections.Generic;

namespace VertigoCase.Core
{
    /// <summary>
    /// Tracks rewards collected during a run, merging stacks by reward Id.
    /// </summary>
    public sealed class RewardInventory
    {
        private readonly List<RewardStack> _items = new List<RewardStack>();

        public IReadOnlyList<RewardStack> Items => _items;

        public event Action<RewardStack> ItemChanged;
        public event Action Cleared;

        public void Add(IRewardDefinition reward, int amount)
        {
            if (reward == null) throw new ArgumentNullException(nameof(reward));
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Must be > 0.");

            for (int i = 0; i < _items.Count; i++)
            {
                if (string.Equals(_items[i].Reward.Id, reward.Id, StringComparison.Ordinal))
                {
                    _items[i] = _items[i].Add(amount);
                    ItemChanged?.Invoke(_items[i]);
                    return;
                }
            }

            var stack = new RewardStack(reward, amount);
            _items.Add(stack);
            ItemChanged?.Invoke(stack);
        }

        public void Clear()
        {
            _items.Clear();
            Cleared?.Invoke();
        }
    }
}
