using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Left-side panel listing every reward collected this run. Entries are pooled and keyed by reward id.
    /// </summary>
    public sealed class CollectedRewardsView : UIViewBase
    {
        [SerializeField] private Transform _listContainer;
        [SerializeField] private RewardEntryView _entryPrefab;
        [SerializeField] private int _prewarm = 8;

        private ObjectPool<RewardEntryView> _pool;
        private readonly Dictionary<string, RewardEntryView> _entries = new();

        public void Initialize()
        {
            _pool ??= new ObjectPool<RewardEntryView>(_entryPrefab, _listContainer, _prewarm);
        }

        public void AddOrUpdate(string rewardId, Sprite icon, int newTotal, bool animate)
        {
            if (!_entries.TryGetValue(rewardId, out RewardEntryView entry))
            {
                entry = _pool.Get();
                entry.transform.SetParent(_listContainer, false);
                entry.EnableMaskable();
                entry.Set(icon, 0);
                _entries[rewardId] = entry;
            }

            if (animate)
                entry.AnimateTo(newTotal);
            else
                entry.Set(icon, newTotal);
        }

        public RectTransform GetEntryTarget(string rewardId)
        {
            return _entries.TryGetValue(rewardId, out RewardEntryView entry)
                ? entry.RectTransform
                : null;
        }

        public void ClearAll()
        {
            foreach (var kvp in _entries)
            {
                kvp.Value.ResetState();
                _pool.Release(kvp.Value);
            }

            _entries.Clear();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_listContainer == null)
                _listContainer = ChildReference.FindChild<Transform>(transform, "ui_container_rewards_list");
        }
    }
}
