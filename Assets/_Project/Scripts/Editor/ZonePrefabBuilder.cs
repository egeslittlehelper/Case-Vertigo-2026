using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Builds zone cell, zone bar, and zone info prefabs with correct hierarchy and components.
    /// </summary>
    public static class ZonePrefabBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/UI";

        [MenuItem("VertigoCase/Build Zone Prefabs")]
        public static void BuildAll()
        {
            EnsureFolder();
            BuildCellPrefab();
            BuildBarPrefab();
            BuildInfoPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ZonePrefabBuilder] All zone prefabs created in " + PrefabFolder);
        }

        private static void BuildCellPrefab()
        {
            string path = PrefabFolder + "/ui_zone_cell.prefab";

            var root = CreateUI("ui_zone_cell");
            root.AddComponent<ZoneCellView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(100f, 80f);

            var textObj = CreateUIChild(root, "ui_text_zone_number_value");
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.raycastTarget = false;
            tmp.maskable = false;
            tmp.text = "1";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 36f;
            tmp.fontStyle = FontStyles.Bold;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildBarPrefab()
        {
            string path = PrefabFolder + "/ui_panel_zone_bar.prefab";
            string cellPrefabPath = PrefabFolder + "/ui_zone_cell.prefab";

            var root = CreateUI("ui_panel_zone_bar");
            var barView = root.AddComponent<ZoneBarView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0f, 1f);
            rootRt.anchorMax = new Vector2(1f, 1f);
            rootRt.pivot = new Vector2(0.5f, 1f);
            rootRt.sizeDelta = new Vector2(0f, 100f);

            var frameObj = CreateUIChild(root, "ui_image_zone_bar_frame");
            var frameImg = frameObj.AddComponent<Image>();
            frameImg.raycastTarget = false;
            frameImg.maskable = false;
            frameImg.type = Image.Type.Sliced;
            var frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            var highlightObj = CreateUIChild(root, "ui_image_zone_current_highlight");
            var highlightBg = highlightObj.AddComponent<Image>();
            highlightBg.raycastTarget = false;
            highlightBg.maskable = false;
            highlightBg.color = Color.white;
            var highlightRt = highlightObj.GetComponent<RectTransform>();
            highlightRt.anchorMin = new Vector2(0.5f, 0f);
            highlightRt.anchorMax = new Vector2(0.5f, 1f);
            highlightRt.pivot = new Vector2(0.5f, 0.5f);
            highlightRt.sizeDelta = new Vector2(100f, 0f);

            var highlightFrameObj = CreateUIChild(highlightObj, "ui_image_zone_current_frame");
            var highlightFrame = highlightFrameObj.AddComponent<Image>();
            highlightFrame.raycastTarget = false;
            highlightFrame.maskable = false;
            highlightFrame.type = Image.Type.Sliced;
            var frameHighlightRt = highlightFrameObj.GetComponent<RectTransform>();
            frameHighlightRt.anchorMin = Vector2.zero;
            frameHighlightRt.anchorMax = Vector2.one;
            frameHighlightRt.sizeDelta = Vector2.zero;

            var maskObj = CreateUIChild(root, "ui_container_zone_bar_mask");
            maskObj.AddComponent<RectMask2D>();
            var maskRt = maskObj.GetComponent<RectTransform>();
            maskRt.anchorMin = Vector2.zero;
            maskRt.anchorMax = Vector2.one;
            maskRt.sizeDelta = Vector2.zero;

            var cellsObj = CreateUIChild(maskObj, "ui_container_zone_cells");
            var cellsRt = cellsObj.GetComponent<RectTransform>();
            cellsRt.anchorMin = new Vector2(0.5f, 0.5f);
            cellsRt.anchorMax = new Vector2(0.5f, 0.5f);
            cellsRt.pivot = new Vector2(0f, 0.5f);
            cellsRt.sizeDelta = Vector2.zero;

            var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cellPrefabPath);
            if (cellPrefab != null)
            {
                var so = new SerializedObject(barView);
                so.FindProperty("_cellPrefab").objectReferenceValue =
                    cellPrefab.GetComponent<ZoneCellView>();
                so.FindProperty("_cellContainer").objectReferenceValue = cellsRt;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildInfoPrefab()
        {
            string path = PrefabFolder + "/ui_panel_zone_info.prefab";

            var root = CreateUI("ui_panel_zone_info");
            root.AddComponent<ZoneInfoView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(1f, 1f);
            rootRt.anchorMax = new Vector2(1f, 1f);
            rootRt.pivot = new Vector2(1f, 1f);
            rootRt.sizeDelta = new Vector2(300f, 180f);

            BuildInfoBadge(root, "super", "SUPER ZONE", "30",
                new Color(1f, 0.84f, 0f, 0.3f), new Vector2(0f, 0f));
            BuildInfoBadge(root, "safe", "SAFE ZONE", "15",
                new Color(0.2f, 0.8f, 0.2f, 0.3f), new Vector2(0f, -85f));

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildInfoBadge(GameObject parent, string key, string label,
            string defaultValue, Color bgColor, Vector2 position)
        {
            var panel = CreateUIChild(parent, $"ui_panel_zone_info_{key}");
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0f, 1f);
            panelRt.anchorMax = new Vector2(1f, 1f);
            panelRt.pivot = new Vector2(0.5f, 1f);
            panelRt.sizeDelta = new Vector2(0f, 75f);
            panelRt.anchoredPosition = position;

            var bgObj = CreateUIChild(panel, $"ui_image_zone_info_{key}_bg");
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.raycastTarget = false;
            bgImg.maskable = false;
            bgImg.color = bgColor;
            bgImg.type = Image.Type.Sliced;
            var bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;

            var labelObj = CreateUIChild(panel, $"ui_text_zone_info_{key}_label");
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.raycastTarget = false;
            labelTmp.maskable = false;
            labelTmp.text = label;
            labelTmp.fontSize = 22f;
            labelTmp.alignment = TextAlignmentOptions.Left;
            var labelRt = labelObj.GetComponent<RectTransform>();
            labelRt.anchorMin = new Vector2(0f, 0f);
            labelRt.anchorMax = new Vector2(0.65f, 1f);
            labelRt.sizeDelta = Vector2.zero;
            labelRt.offsetMin = new Vector2(10f, 0f);

            var valueObj = CreateUIChild(panel, $"ui_text_zone_info_{key}_value");
            var valueTmp = valueObj.AddComponent<TextMeshProUGUI>();
            valueTmp.raycastTarget = false;
            valueTmp.maskable = false;
            valueTmp.text = defaultValue;
            valueTmp.fontSize = 32f;
            valueTmp.fontStyle = FontStyles.Bold;
            valueTmp.alignment = TextAlignmentOptions.Right;
            var valueRt = valueObj.GetComponent<RectTransform>();
            valueRt.anchorMin = new Vector2(0.65f, 0f);
            valueRt.anchorMax = new Vector2(1f, 1f);
            valueRt.sizeDelta = Vector2.zero;
            valueRt.offsetMax = new Vector2(-10f, 0f);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Prefabs"))
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Prefabs", "UI");
        }

        private static GameObject CreateUI(string name)
        {
            return new GameObject(name, typeof(RectTransform));
        }

        private static GameObject CreateUIChild(GameObject parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            return go;
        }
    }
}
