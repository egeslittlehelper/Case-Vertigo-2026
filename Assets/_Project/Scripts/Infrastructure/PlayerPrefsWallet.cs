using System;
using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Infrastructure
{
    /// <summary>
    /// Persistent wallet backed by PlayerPrefs. Balance survives app restarts.
    /// </summary>
    public sealed class PlayerPrefsWallet : IWallet
    {
        private const string Key = "wallet_balance";

        public int Balance { get; private set; }

        public event Action<int> BalanceChanged;

        public PlayerPrefsWallet(int defaultBalance = 0)
        {
            Balance = PlayerPrefs.GetInt(Key, Math.Max(0, defaultBalance));
        }

        public bool CanAfford(int amount) => amount >= 0 && Balance >= amount;

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount)) return false;
            Balance -= amount;
            Save();
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Balance += amount;
            Save();
            BalanceChanged?.Invoke(Balance);
        }

        private void Save()
        {
            PlayerPrefs.SetInt(Key, Balance);
            PlayerPrefs.Save();
        }
    }
}
