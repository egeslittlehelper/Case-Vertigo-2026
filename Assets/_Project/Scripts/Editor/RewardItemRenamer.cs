using UnityEditor;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Strips UI prefixes from RewardItemSO asset names and normalizes to lowercase reward_ convention.
    /// </summary>
    public static class RewardItemRenamer
    {
        [MenuItem("VertigoCase/Clean Reward Item Names")]
        public static void CleanNames()
        {
            string[] guids = AssetDatabase.FindAssets("t:RewardItemSO");
            int renamed = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var reward = AssetDatabase.LoadAssetAtPath<RewardItemSO>(path);
                if (reward == null) continue;

                string oldName = reward.name;
                string newName = CleanName(oldName);

                if (oldName != newName)
                {
                    string result = AssetDatabase.RenameAsset(path, newName);
                    if (string.IsNullOrEmpty(result))
                    {
                        Debug.Log($"Renamed: {oldName} → {newName}");
                        renamed++;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to rename {oldName}: {result}");
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Done. Renamed {renamed} reward items.");
        }

        private static string CleanName(string name)
        {
            string clean = name;

            // Strip common UI prefixes (case-insensitive match, preserve rest)
            string lower = clean.ToLowerInvariant();
            if (lower.StartsWith("ui_icons_"))
                clean = clean.Substring("ui_icons_".Length);
            else if (lower.StartsWith("ui_icon_renders_"))
                clean = clean.Substring("ui_icon_renders_".Length);
            else if (lower.StartsWith("ui_icon_render_"))
                clean = clean.Substring("ui_icon_render_".Length);
            else if (lower.StartsWith("ui_icon_"))
                clean = clean.Substring("ui_icon_".Length);
            else if (lower.StartsWith("ui_"))
                clean = clean.Substring("ui_".Length);

            clean = clean.ToLowerInvariant();

            // Add reward_ prefix if not already there
            if (!clean.StartsWith("reward_"))
                clean = "reward_" + clean;

            return clean;
        }
    }
}
