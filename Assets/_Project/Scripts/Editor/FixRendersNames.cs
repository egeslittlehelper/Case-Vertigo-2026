using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Editor
{
    /// <summary>
    /// One-time fix: adds "renders_" back to reward items that had it stripped.
    /// </summary>
    public static class FixRendersNames
    {
        private static readonly HashSet<string> AffectedNames = new HashSet<string>
        {
            "reward_tier1_shotgun",
            "reward_tier2_mle",
            "reward_tier2_rifle",
            "reward_tier3_shotgun",
            "reward_tier3_smg",
            "reward_tier3_sniper"
        };

        [MenuItem("VertigoCase/Fix Renders Names (one-time)")]
        public static void Fix()
        {
            string[] guids = AssetDatabase.FindAssets("t:RewardItemSO");
            int fixed_count = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var reward = AssetDatabase.LoadAssetAtPath<RewardItemSO>(path);
                if (reward == null) continue;

                if (AffectedNames.Contains(reward.name))
                {
                    string newName = reward.name.Replace("reward_tier", "reward_renders_tier");
                    string result = AssetDatabase.RenameAsset(path, newName);
                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.Log($"Fixed: {reward.name} → {newName}");
                        fixed_count++;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to rename {reward.name}: {result}");
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Done. Fixed {fixed_count} reward items.");
        }
    }
}
