using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Builds reward entry, fly icon, and collected rewards panel prefabs.
    /// </summary>
    public static class RewardsPrefabBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/UI";

        [MenuItem("VertigoCase/Build Prefabs/Rewards - All")]
        public static void BuildAll()
        {
            EnsureFolder();
            BuildFlyIconPrefab();
            BuildEntryPrefab();
            BuildPanelPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[RewardsPrefabBuilder] All reward prefabs created in " + PrefabFolder);
        }

        [MenuItem("VertigoCase/Build Prefabs/Rewards - Fly Icon")]
        public static void BuildFlyIcon() { EnsureFolder(); BuildFlyIconPrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Rewards - Entry")]
        public static void BuildEntry() { EnsureFolder(); BuildEntryPrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Rewards - Panel")]
        public static void BuildPanel() { EnsureFolder(); BuildPanelPrefab(); AssetDatabase.SaveAssets(); }

        private static void BuildFlyIconPrefab()
        {
            string path = PrefabFolder + "/ui_fly_icon.prefab";

            var root = CreateUI("ui_fly_icon");
            root.AddComponent<FlyIconView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(48f, 48f);

            var iconObj = CreateUIChild(root, "ui_image_fly_icon_value");
            var img = iconObj.AddComponent<Image>();
            img.raycastTarget = false;
            img.maskable = false;
            img.preserveAspect = true;
            var iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = Vector2.zero;
            iconRt.anchorMax = Vector2.one;
            iconRt.sizeDelta = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildEntryPrefab()
        {
            string path = PrefabFolder + "/ui_reward_entry.prefab";

            var root = CreateUI("ui_reward_entry");
            root.AddComponent<RewardEntryView>();
            var le = root.AddComponent<LayoutElement>();
            le.preferredHeight = 80f;
            le.flexibleWidth = 1f;
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.sizeDelta = new Vector2(0f, 80f);

            var animObj = CreateUIChild(root, "ui_anim_reward_entry");
            var animRt = animObj.GetComponent<RectTransform>();
            animRt.anchorMin = Vector2.zero;
            animRt.anchorMax = Vector2.one;
            animRt.sizeDelta = Vector2.zero;

            // Icon — anchor-based, left 45% of entry, with padding
            var iconObj = CreateUIChild(animObj, "ui_image_reward_icon_value");
            var iconImg = iconObj.AddComponent<Image>();
            iconImg.raycastTarget = false;
            iconImg.maskable = true;
            iconImg.preserveAspect = true;
            var iconRt = iconObj.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0.05f, 0.05f);
            iconRt.anchorMax = new Vector2(0.45f, 0.95f);
            iconRt.sizeDelta = Vector2.zero;
            iconRt.anchoredPosition = Vector2.zero;

            // Amount — right 50%, left-aligned, with left padding for gap from icon
            var textObj = CreateUIChild(animObj, "ui_text_reward_amount_value");
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.raycastTarget = false;
            tmp.maskable = true;
            tmp.text = "0";
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmp.fontSize = 36f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 14f;
            tmp.fontSizeMax = 36f;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0.5f, 0f);
            textRt.anchorMax = new Vector2(0.98f, 1f);
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildPanelPrefab()
        {
            string path = PrefabFolder + "/ui_panel_rewards.prefab";
            string entryPrefabPath = PrefabFolder + "/ui_reward_entry.prefab";

            // Panel — fixed size anchored top-left, scales with screen via anchors
            var root = CreateUI("ui_panel_rewards");
            var rewardsView = root.AddComponent<CollectedRewardsView>();
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0f, 0.05f);
            rootRt.anchorMax = new Vector2(0.18f, 0.85f);
            rootRt.pivot = new Vector2(0f, 1f);
            rootRt.sizeDelta = Vector2.zero;
            rootRt.offsetMin = new Vector2(12f, 0f);
            rootRt.offsetMax = new Vector2(0f, 0f);

            // Frame — solid sprite, sliced (user assigns the sprite in Editor)
            var frameObj = CreateUIChild(root, "ui_image_rewards_frame");
            var frameImg = frameObj.AddComponent<Image>();
            frameImg.raycastTarget = false;
            frameImg.maskable = false;
            frameImg.type = Image.Type.Sliced;
            frameImg.color = Color.white;
            var frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            // Scroll area
            var scrollObj = CreateUIChild(root, "ui_container_rewards_scroll");
            var scrollRt = scrollObj.GetComponent<RectTransform>();
            scrollRt.anchorMin = Vector2.zero;
            scrollRt.anchorMax = Vector2.one;
            scrollRt.offsetMin = new Vector2(6f, 6f);
            scrollRt.offsetMax = new Vector2(-16f, -6f);

            // Viewport — separate child with RectMask2D for clipping
            var viewportObj = CreateUIChild(scrollObj, "ui_viewport_rewards");
            viewportObj.AddComponent<RectMask2D>();
            var viewportRt = viewportObj.GetComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.sizeDelta = Vector2.zero;

            // Vertical list — grows with content, scrolls when it overflows
            var listObj = CreateUIChild(viewportObj, "ui_container_rewards_list");
            var listRt = listObj.GetComponent<RectTransform>();
            listRt.anchorMin = new Vector2(0f, 1f);
            listRt.anchorMax = new Vector2(1f, 1f);
            listRt.pivot = new Vector2(0.5f, 1f);
            listRt.sizeDelta = new Vector2(0f, 0f);
            listRt.anchoredPosition = Vector2.zero;

            var layout = listObj.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.padding = new RectOffset(4, 4, 4, 4);
            layout.spacing = 10f;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var listFitter = listObj.AddComponent<ContentSizeFitter>();
            listFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollRect = scrollObj.AddComponent<ScrollRect>();
            scrollRect.content = listRt;
            scrollRect.viewport = viewportRt;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 20f;

            // Vertical scrollbar — thin bar on right edge of panel
            var scrollbarObj = CreateUIChild(root, "ui_scrollbar_rewards");
            var scrollbarImg = scrollbarObj.AddComponent<Image>();
            scrollbarImg.raycastTarget = true;
            scrollbarImg.maskable = false;
            scrollbarImg.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);
            var scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            var scrollbarRt = scrollbarObj.GetComponent<RectTransform>();
            scrollbarRt.anchorMin = new Vector2(1f, 0f);
            scrollbarRt.anchorMax = new Vector2(1f, 1f);
            scrollbarRt.pivot = new Vector2(1f, 0.5f);
            scrollbarRt.sizeDelta = new Vector2(10f, 0f);
            scrollbarRt.offsetMin = new Vector2(-10f, 6f);
            scrollbarRt.offsetMax = new Vector2(0f, -6f);

            var slidingArea = CreateUIChild(scrollbarObj, "ui_scrollbar_rewards_sliding");
            var slidingRt = slidingArea.GetComponent<RectTransform>();
            slidingRt.anchorMin = Vector2.zero;
            slidingRt.anchorMax = Vector2.one;
            slidingRt.sizeDelta = Vector2.zero;

            var handleObj = CreateUIChild(slidingArea, "ui_scrollbar_rewards_handle");
            var handleImg = handleObj.AddComponent<Image>();
            handleImg.raycastTarget = true;
            handleImg.maskable = false;
            handleImg.color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
            var handleRt = handleObj.GetComponent<RectTransform>();
            handleRt.anchorMin = Vector2.zero;
            handleRt.anchorMax = new Vector2(1f, 0.2f);
            handleRt.sizeDelta = Vector2.zero;

            scrollbar.handleRect = handleRt;
            scrollbar.targetGraphic = handleImg;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            var entryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(entryPrefabPath);
            if (entryPrefab != null)
            {
                var so = new SerializedObject(rewardsView);
                so.FindProperty("_entryPrefab").objectReferenceValue =
                    entryPrefab.GetComponent<RewardEntryView>();
                so.FindProperty("_listContainer").objectReferenceValue = listRt;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
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
