using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// Central registry of all reward items — UI uses it to look up icons by Id.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Reward Catalog")]
    public sealed class RewardCatalogSO : ScriptableObject
    {
        [SerializeField] private List<RewardItemSO> _rewards = new List<RewardItemSO>();

        private Dictionary<string, RewardItemSO> _lookup;

        public IReadOnlyList<RewardItemSO> Rewards => _rewards;

        public bool TryGetReward(string id, out RewardItemSO reward)
        {
            EnsureLookup();
            return _lookup.TryGetValue(id, out reward);
        }

        public bool TryGetIcon(string id, out Sprite icon)
        {
            if (TryGetReward(id, out RewardItemSO reward) && reward.Icon != null)
            {
                icon = reward.Icon;
                return true;
            }

            icon = null;
            return false;
        }

        private void EnsureLookup()
        {
            if (_lookup != null && _lookup.Count == _rewards.Count)
                return;

            _lookup = new Dictionary<string, RewardItemSO>(_rewards.Count);
            for (int i = 0; i < _rewards.Count; i++)
            {
                if (_rewards[i] != null && !string.IsNullOrEmpty(_rewards[i].Id))
                    _lookup[_rewards[i].Id] = _rewards[i];
            }
        }
    }
}
