using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Non-persistent wallet for tests and default fallback.
    /// </summary>
    public sealed class InMemoryWallet : IWallet
    {
        public int Balance { get; private set; }

        public event Action<int> BalanceChanged;

        public InMemoryWallet(int startingBalance = 0)
        {
            Balance = Math.Max(0, startingBalance);
        }

        public bool CanAfford(int amount) => amount >= 0 && Balance >= amount;

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount)) return false;
            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }
    }
}
