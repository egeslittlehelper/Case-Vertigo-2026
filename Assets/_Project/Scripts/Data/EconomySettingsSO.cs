using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Economy tuning: revive costs, starting balance, and which reward acts as currency.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Economy Settings")]
    public sealed class EconomySettingsSO : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField] private RewardItemSO _currencyReward;
        [SerializeField] private int _startingBalance = 100;

        [Header("Revive")]
        [SerializeField] [Min(1)] private int _reviveBaseCost = 25;
        [SerializeField] [Min(1f)] private float _reviveCostMultiplier = 2f;

        public RewardItemSO CurrencyReward => _currencyReward;
        public string CurrencyId => _currencyReward != null ? _currencyReward.Id : "";
        public int StartingBalance => _startingBalance;
        public int ReviveBaseCost => _reviveBaseCost;
        public float ReviveCostMultiplier => _reviveCostMultiplier;

        public IReviveCostPolicy CreateReviveCostPolicy()
        {
            return new EscalatingReviveCost(_reviveBaseCost, _reviveCostMultiplier);
        }
    }
}
