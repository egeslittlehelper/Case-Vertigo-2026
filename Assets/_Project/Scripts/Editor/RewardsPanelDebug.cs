using UnityEditor;
using UnityEngine;
using VertigoCase.Data;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    public static class RewardsPanelDebug
    {
        [MenuItem("VertigoCase/Debug/Fill Rewards Panel")]
        private static void FillRewardsPanel()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[RewardsPanelDebug] Enter Play mode first.");
                return;
            }

            var view = Object.FindFirstObjectByType<CollectedRewardsView>();
            if (view == null)
            {
                Debug.LogError("[RewardsPanelDebug] No CollectedRewardsView found in scene.");
                return;
            }

            var catalogs = Resources.FindObjectsOfTypeAll<RewardCatalogSO>();
            if (catalogs.Length == 0)
            {
                Debug.LogError("[RewardsPanelDebug] No RewardCatalogSO found.");
                return;
            }

            var catalog = catalogs[0];
            view.Initialize();

            for (int i = 0; i < catalog.Rewards.Count; i++)
            {
                var reward = catalog.Rewards[i];
                if (reward == null) continue;

                int amount = Random.Range(1, 10000);
                catalog.TryGetIcon(reward.Id, out Sprite icon);
                view.AddOrUpdate(reward.Id, icon, amount, false);
            }

            Debug.Log($"[RewardsPanelDebug] Added {catalog.Rewards.Count} rewards to panel.");
        }
    }
}
