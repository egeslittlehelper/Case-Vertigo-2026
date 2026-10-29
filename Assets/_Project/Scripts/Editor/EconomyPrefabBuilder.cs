using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Builds economy-related prefabs (wallet panel).
    /// </summary>
    public static class EconomyPrefabBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/UI";

        [MenuItem("VertigoCase/Build Prefabs/Economy - All")]
        public static void BuildAll()
        {
            EnsureFolder();
            BuildWalletPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[EconomyPrefabBuilder] All economy prefabs created in " + PrefabFolder);
        }

        [MenuItem("VertigoCase/Build Prefabs/Economy - Wallet")]
        public static void BuildWallet() { EnsureFolder(); BuildWalletPrefab(); AssetDatabase.SaveAssets(); }

        private static void BuildWalletPrefab()
        {
            string path = PrefabFolder + "/ui_panel_wallet.prefab";

            var root = new GameObject("ui_panel_wallet", typeof(RectTransform));
            root.AddComponent<WalletView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0.7f, 0.92f);
            rootRt.anchorMax = new Vector2(0.98f, 0.98f);
            rootRt.sizeDelta = Vector2.zero;

            var bgObj = CreateChild(root, "ui_image_wallet_bg");
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.raycastTarget = false;
            bgImg.maskable = false;
            bgImg.type = Image.Type.Sliced;
            bgImg.color = new Color(0.12f, 0.12f, 0.16f, 0.85f);
            var bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;

            var iconObj = CreateChild(root, "ui_image_wallet_icon");
            var iconImg = iconObj.AddComponent<Image>();
            iconImg.raycastTarget = false;
            iconImg.maskable = false;
            iconImg.preserveAspect = true;
            var iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0.02f, 0.1f);
            iconRt.anchorMax = new Vector2(0.22f, 0.9f);
            iconRt.sizeDelta = Vector2.zero;

            var textObj = CreateChild(root, "ui_text_wallet_balance_value");
            var textTmp = textObj.AddComponent<TextMeshProUGUI>();
            textTmp.raycastTarget = false;
            textTmp.maskable = false;
            textTmp.text = "0";
            textTmp.alignment = TextAlignmentOptions.MidlineRight;
            textTmp.fontSize = 28f;
            textTmp.fontStyle = FontStyles.Bold;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.24f, 0f);
            textRt.anchorMax = new Vector2(0.96f, 1f);
            textRt.sizeDelta = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[EconomyPrefabBuilder] Created " + path);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Prefabs"))
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Prefabs", "UI");
        }

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            return go;
        }
    }
}
