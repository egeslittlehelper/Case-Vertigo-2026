using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Finds every RewardItemSO in the project and fills the catalog's list.
    /// </summary>
    public static class RewardCatalogPopulator
    {
        [MenuItem("VertigoCase/Populate Reward Catalog")]
        public static void Populate()
        {
            string[] catalogGuids = AssetDatabase.FindAssets("t:RewardCatalogSO");
            if (catalogGuids.Length == 0)
            {
                Debug.LogError("No RewardCatalogSO asset found. Create one first.");
                return;
            }

            string catalogPath = AssetDatabase.GUIDToAssetPath(catalogGuids[0]);
            var catalog = AssetDatabase.LoadAssetAtPath<RewardCatalogSO>(catalogPath);

            string[] rewardGuids = AssetDatabase.FindAssets("t:RewardItemSO");
            var rewards = new List<RewardItemSO>(rewardGuids.Length);

            foreach (string guid in rewardGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var reward = AssetDatabase.LoadAssetAtPath<RewardItemSO>(path);
                if (reward != null)
                    rewards.Add(reward);
            }

            rewards.Sort((a, b) => string.Compare(a.Id, b.Id, System.StringComparison.Ordinal));

            var field = typeof(RewardCatalogSO).GetField("_rewards",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Undo.RecordObject(catalog, "Populate Reward Catalog");
            field.SetValue(catalog, rewards);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();

            Debug.Log($"Reward catalog populated with {rewards.Count} items.");
        }
    }
}
