using System;

namespace VertigoCase.Core
{
    /// <summary>
    /// Abstraction for a currency wallet — swappable between in-memory (tests) and PlayerPrefs (production).
    /// </summary>
    public interface IWallet
    {
        int Balance { get; }
        bool CanAfford(int amount);
        bool TrySpend(int amount);
        void Add(int amount);
        event Action<int> BalanceChanged;
    }
}
