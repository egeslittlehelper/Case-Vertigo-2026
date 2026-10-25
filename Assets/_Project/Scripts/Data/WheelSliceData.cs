using System;
using UnityEngine;
using VertigoCase.Core;

namespace VertigoCase.Data
{
    /// <summary>
    /// Inspector-editable slice — implements ISliceDefinition for the domain.
    /// </summary>
    [Serializable]
    public sealed class WheelSliceData : ISliceDefinition
    {
        [SerializeField] private RewardItemSO _reward;
        [SerializeField] [Min(0)] private int _baseAmount = 100;
        [SerializeField] [Min(1)] private int _weight = 10;
        [SerializeField] private bool _isBomb;

        public IRewardDefinition Reward => _reward;
        public int BaseAmount => _baseAmount;
        public int Weight => _weight;
        public bool IsBomb => _isBomb;

        public WheelSliceData() { }

        public WheelSliceData(RewardItemSO reward, int baseAmount, int weight, bool isBomb)
        {
            _reward = reward;
            _baseAmount = baseAmount;
            _weight = weight;
            _isBomb = isBomb;
        }
    }
}
