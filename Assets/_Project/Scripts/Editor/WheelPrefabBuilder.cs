using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Builds the ui_wheel_slice and ui_panel_wheel prefabs with correct hierarchy and components.
    /// </summary>
    public static class WheelPrefabBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/UI";

        [MenuItem("VertigoCase/Build Prefabs/Wheel - All")]
        public static void BuildAll()
        {
            EnsureFolder();
            BuildSlicePrefab();
            BuildWheelPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[WheelPrefabBuilder] Both prefabs created in " + PrefabFolder);
        }

        [MenuItem("VertigoCase/Build Prefabs/Wheel - Slice")]
        public static void BuildSlice() { EnsureFolder(); BuildSlicePrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Wheel - Panel")]
        public static void BuildWheel() { EnsureFolder(); BuildWheelPrefab(); AssetDatabase.SaveAssets(); }

        private static void BuildSlicePrefab()
        {
            string path = PrefabFolder + "/ui_wheel_slice.prefab";

            var root = CreateUIObject("ui_wheel_slice");
            root.AddComponent<WheelSliceView>();

            var content = CreateUIChild(root, "ui_container_slice_content");
            var contentRt = content.GetComponent<RectTransform>();
            contentRt.sizeDelta = new Vector2(120f, 140f);
            contentRt.pivot = new Vector2(0.5f, 0.5f);

            var iconObj = CreateUIChild(content, "ui_image_slice_icon_value");
            var icon = iconObj.AddComponent<Image>();
            icon.raycastTarget = false;
            icon.maskable = false;
            icon.preserveAspect = true;
            var iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0.5f, 0.5f);
            iconRt.anchorMax = new Vector2(0.5f, 0.5f);
            iconRt.pivot = new Vector2(0.5f, 0.5f);
            iconRt.sizeDelta = new Vector2(90f, 90f);
            iconRt.anchoredPosition = Vector2.zero;

            var textObj = CreateUIChild(content, "ui_text_slice_amount_value");
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.raycastTarget = false;
            tmp.maskable = false;
            tmp.text = "x100";
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 6f;
            tmp.fontSizeMax = 18f;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.5f, 0.5f);
            textRt.anchorMax = new Vector2(0.5f, 0.5f);
            textRt.pivot = new Vector2(0.5f, 0.5f);
            textRt.sizeDelta = new Vector2(100f, 24f);
            textRt.anchoredPosition = new Vector2(0f, -52f);

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[WheelPrefabBuilder] Created " + path);
        }

        private static void BuildWheelPrefab()
        {
            string path = PrefabFolder + "/ui_panel_wheel.prefab";
            string slicePrefabPath = PrefabFolder + "/ui_wheel_slice.prefab";

            var root = CreateUIObject("ui_panel_wheel");
            var wheelView = root.AddComponent<WheelView>();
            root.AddComponent<SliceViewFactory>();

            var titleObj = CreateUIChild(root, "ui_text_wheel_title_value");
            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.raycastTarget = false;
            titleTmp.maskable = false;
            titleTmp.text = "SPIN";
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.fontSize = 36f;
            var titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 1f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.sizeDelta = new Vector2(0f, 60f);
            titleRt.anchoredPosition = new Vector2(0f, 0f);

            var animObj = CreateUIChild(root, "ui_anim_wheel");
            var animRt = animObj.GetComponent<RectTransform>();
            animRt.anchorMin = new Vector2(0.5f, 0.5f);
            animRt.anchorMax = new Vector2(0.5f, 0.5f);
            animRt.sizeDelta = new Vector2(900f, 900f);

            var baseObj = CreateUIChild(animObj, "ui_image_wheel_base_value");
            var baseImg = baseObj.AddComponent<Image>();
            baseImg.raycastTarget = false;
            baseImg.maskable = false;
            baseImg.preserveAspect = true;
            var baseRt = baseObj.GetComponent<RectTransform>();
            baseRt.anchorMin = Vector2.zero;
            baseRt.anchorMax = Vector2.one;
            baseRt.sizeDelta = Vector2.zero;

            var slicesObj = CreateUIChild(animObj, "ui_container_wheel_slices");
            var slicesRt = slicesObj.GetComponent<RectTransform>();
            slicesRt.anchorMin = new Vector2(0.5f, 0.5f);
            slicesRt.anchorMax = new Vector2(0.5f, 0.5f);
            slicesRt.sizeDelta = Vector2.zero;

            var indicatorObj = CreateUIChild(root, "ui_image_wheel_indicator_value");
            var indicatorImg = indicatorObj.AddComponent<Image>();
            indicatorImg.raycastTarget = false;
            indicatorImg.maskable = false;
            indicatorImg.preserveAspect = true;
            var indicatorRt = indicatorObj.GetComponent<RectTransform>();
            indicatorRt.anchorMin = new Vector2(0.5f, 0.5f);
            indicatorRt.anchorMax = new Vector2(0.5f, 0.5f);
            indicatorRt.pivot = new Vector2(0.5f, 0.5f);
            indicatorRt.sizeDelta = new Vector2(80f, 80f);
            indicatorRt.anchoredPosition = new Vector2(0f, 460f);

            var subtitleObj = CreateUIChild(root, "ui_text_wheel_subtitle_value");
            var subTmp = subtitleObj.AddComponent<TextMeshProUGUI>();
            subTmp.raycastTarget = false;
            subTmp.maskable = false;
            subTmp.text = "";
            subTmp.alignment = TextAlignmentOptions.Center;
            subTmp.fontSize = 20f;

            var buttonObj = CreateUIChild(root, "ui_button_spin");
            var buttonImg = buttonObj.AddComponent<Image>();
            buttonImg.raycastTarget = true;
            buttonImg.maskable = false;
            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImg;
            var buttonRt = buttonObj.GetComponent<RectTransform>();
            buttonRt.anchorMin = new Vector2(0.5f, 0f);
            buttonRt.anchorMax = new Vector2(0.5f, 0f);
            buttonRt.pivot = new Vector2(0.5f, 0f);
            buttonRt.sizeDelta = new Vector2(200f, 60f);
            buttonRt.anchoredPosition = new Vector2(0f, 20f);

            var labelObj = CreateUIChild(buttonObj, "ui_text_spin_label");
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.raycastTarget = false;
            labelTmp.maskable = false;
            labelTmp.text = "SPIN";
            labelTmp.alignment = TextAlignmentOptions.Center;
            labelTmp.fontSize = 28f;
            var labelRt = labelObj.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;

            var slicePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(slicePrefabPath);
            if (slicePrefab != null)
            {
                var factory = root.GetComponent<SliceViewFactory>();
                var so = new SerializedObject(factory);
                so.FindProperty("_slicePrefab").objectReferenceValue =
                    slicePrefab.GetComponent<WheelSliceView>();
                so.FindProperty("_container").objectReferenceValue = slicesRt;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

#if UNITY_EDITOR
            root.AddComponent<WheelViewSandbox>();
#endif

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[WheelPrefabBuilder] Created " + path);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Prefabs"))
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
                AssetDatabase.CreateFolder("Assets/_Project/Prefabs", "UI");
        }

        private static GameObject CreateUIObject(string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            return go;
        }

        private static GameObject CreateUIChild(GameObject parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            return go;
        }
    }
}
