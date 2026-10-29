using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Builds bomb popup, cash-out popup, and exit button prefabs.
    /// </summary>
    public static class FlowPrefabBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/UI";
        private const string ButtonSpritePath = "Assets/_Project/Art/Sprites/UI_button_grey_standard.png";

        private static Sprite LoadButtonSprite()
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ButtonSpritePath);
        }

        [MenuItem("VertigoCase/Build Prefabs/Flow - All")]
        public static void BuildAll()
        {
            EnsureFolder();
            BuildExitButtonPrefab();
            BuildBombPopupPrefab();
            BuildConfirmLossPrefab();
            BuildCashOutPopupPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[FlowPrefabBuilder] All flow prefabs created in " + PrefabFolder);
        }

        [MenuItem("VertigoCase/Build Prefabs/Flow - Exit Button")]
        public static void BuildExit() { EnsureFolder(); BuildExitButtonPrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Flow - Bomb Popup")]
        public static void BuildBomb() { EnsureFolder(); BuildBombPopupPrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Flow - Confirm Loss")]
        public static void BuildConfirm() { EnsureFolder(); BuildConfirmLossPrefab(); AssetDatabase.SaveAssets(); }

        [MenuItem("VertigoCase/Build Prefabs/Flow - Cash Out Popup")]
        public static void BuildCashOut() { EnsureFolder(); BuildCashOutPopupPrefab(); AssetDatabase.SaveAssets(); }

        private static void BuildExitButtonPrefab()
        {
            string path = PrefabFolder + "/ui_button_exit.prefab";

            var root = CreateUI("ui_button_exit");
            root.AddComponent<ExitButtonView>();
            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;

            var img = root.AddComponent<Image>();
            img.raycastTarget = true;
            img.maskable = false;
            img.type = Image.Type.Sliced;
            img.sprite = LoadButtonSprite();
            var btn = root.AddComponent<Button>();
            btn.targetGraphic = img;

            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0f, 0f);
            rootRt.anchorMax = new Vector2(1f, 0f);
            rootRt.pivot = new Vector2(0.5f, 0f);
            rootRt.sizeDelta = new Vector2(0f, 80f);
            rootRt.anchoredPosition = new Vector2(0f, 8f);

            var labelObj = CreateUIChild(root, "ui_text_exit_label");
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.raycastTarget = false;
            labelTmp.maskable = false;
            labelTmp.text = "EXIT";
            labelTmp.alignment = TextAlignmentOptions.Center;
            labelTmp.fontSize = 36f;
            labelTmp.fontStyle = FontStyles.Bold;
            var labelRt = labelObj.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[FlowPrefabBuilder] Created " + path);
        }

        private static void BuildBombPopupPrefab()
        {
            string path = PrefabFolder + "/ui_popup_bomb.prefab";

            var root = CreateUI("ui_popup_bomb");
            root.AddComponent<BombPopupView>();
            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            // Full-screen dim — rewards panel renders on top via hierarchy order
            var dimObj = CreateUIChild(root, "ui_image_popup_bomb_dim");
            var dimImg = dimObj.AddComponent<Image>();
            dimImg.raycastTarget = true;
            dimImg.maskable = false;
            dimImg.color = new Color(0f, 0f, 0f, 0.7f);
            dimObj.AddComponent<RaycastBlocker>();
            var dimRt = dimObj.GetComponent<RectTransform>();
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.sizeDelta = Vector2.zero;

            // Popup panel — centered in available space (right of rewards)
            var animObj = CreateUIChild(root, "ui_anim_popup_bomb");
            var animRt = animObj.GetComponent<RectTransform>();
            animRt.anchorMin = new Vector2(0.24f, 0.1f);
            animRt.anchorMax = new Vector2(0.95f, 0.9f);
            animRt.sizeDelta = Vector2.zero;

            // Dark frame background (reference uses dark panel)
            var frameObj = CreateUIChild(animObj, "ui_image_popup_bomb_frame");
            var frameImg = frameObj.AddComponent<Image>();
            frameImg.raycastTarget = false;
            frameImg.maskable = false;
            frameImg.type = Image.Type.Sliced;
            frameImg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
            var frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            // Title — top area, large bold white text
            var titleObj = CreateUIChild(animObj, "ui_text_popup_bomb_title");
            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.raycastTarget = false;
            titleTmp.maskable = false;
            titleTmp.text = "OH NO, A BOMB EXPLODED\nRIGHT IN YOUR HANDS!";
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.enableAutoSizing = true;
            titleTmp.fontSizeMin = 20f;
            titleTmp.fontSizeMax = 80f;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.color = Color.white;
            var titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.05f, 0.82f);
            titleRt.anchorMax = new Vector2(0.95f, 0.97f);
            titleRt.sizeDelta = Vector2.zero;

            // Subtitle — "Revive yourself to keep your rewards." / lost count
            var lostObj = CreateUIChild(animObj, "ui_text_popup_bomb_lost_value");
            var lostTmp = lostObj.AddComponent<TextMeshProUGUI>();
            lostTmp.raycastTarget = false;
            lostTmp.maskable = false;
            lostTmp.text = "Revive yourself to keep your rewards.";
            lostTmp.alignment = TextAlignmentOptions.Center;
            lostTmp.enableAutoSizing = true;
            lostTmp.fontSizeMin = 14f;
            lostTmp.fontSizeMax = 48f;
            lostTmp.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            var lostRt = lostObj.GetComponent<RectTransform>();
            lostRt.anchorMin = new Vector2(0.08f, 0.74f);
            lostRt.anchorMax = new Vector2(0.92f, 0.83f);
            lostRt.sizeDelta = Vector2.zero;

            // Bomb card image — large, centered, preserve aspect
            var cardObj = CreateUIChild(animObj, "ui_image_popup_bomb_card");
            var cardImg = cardObj.AddComponent<Image>();
            cardImg.raycastTarget = false;
            cardImg.maskable = false;
            cardImg.preserveAspect = true;
            var cardRt = cardObj.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.2f, 0.22f);
            cardRt.anchorMax = new Vector2(0.8f, 0.73f);
            cardRt.sizeDelta = Vector2.zero;

            // Give Up button — left side at bottom
            var giveUpObj = CreateUIChild(animObj, "ui_button_popup_bomb_give_up");
            var giveUpImg = giveUpObj.AddComponent<Image>();
            giveUpImg.raycastTarget = true;
            giveUpImg.maskable = false;
            giveUpImg.type = Image.Type.Sliced;
            giveUpImg.sprite = LoadButtonSprite();
            giveUpImg.color = Color.white;
            var giveUpBtn = giveUpObj.AddComponent<Button>();
            giveUpBtn.targetGraphic = giveUpImg;
            var giveUpRt = giveUpObj.GetComponent<RectTransform>();
            giveUpRt.anchorMin = new Vector2(0.05f, 0.04f);
            giveUpRt.anchorMax = new Vector2(0.48f, 0.18f);
            giveUpRt.sizeDelta = Vector2.zero;

            var giveUpLabel = CreateUIChild(giveUpObj, "ui_text_popup_bomb_give_up_label");
            var giveUpTmp = giveUpLabel.AddComponent<TextMeshProUGUI>();
            giveUpTmp.raycastTarget = false;
            giveUpTmp.maskable = false;
            giveUpTmp.text = "GIVE UP";
            giveUpTmp.alignment = TextAlignmentOptions.Center;
            giveUpTmp.enableAutoSizing = true;
            giveUpTmp.fontSizeMin = 16f;
            giveUpTmp.fontSizeMax = 64f;
            giveUpTmp.fontStyle = FontStyles.Bold;
            giveUpTmp.color = Color.white;
            var giveUpLabelRt = giveUpLabel.GetComponent<RectTransform>();
            giveUpLabelRt.anchorMin = Vector2.zero;
            giveUpLabelRt.anchorMax = Vector2.one;
            giveUpLabelRt.sizeDelta = Vector2.zero;

            // Revive button — right side at bottom, green (hidden until CP11)
            var reviveObj = CreateUIChild(animObj, "ui_button_popup_bomb_revive");
            var reviveImg = reviveObj.AddComponent<Image>();
            reviveImg.raycastTarget = true;
            reviveImg.maskable = false;
            reviveImg.type = Image.Type.Sliced;
            reviveImg.sprite = LoadButtonSprite();
            reviveImg.color = new Color(0.2f, 0.7f, 0.2f, 1f);
            var reviveBtn = reviveObj.AddComponent<Button>();
            reviveBtn.targetGraphic = reviveImg;
            var reviveRt = reviveObj.GetComponent<RectTransform>();
            reviveRt.anchorMin = new Vector2(0.52f, 0.04f);
            reviveRt.anchorMax = new Vector2(0.95f, 0.18f);
            reviveRt.sizeDelta = Vector2.zero;

            // "REVIVE" label — upper portion of button
            var reviveLabel = CreateUIChild(reviveObj, "ui_text_popup_bomb_revive_label");
            var reviveTmp = reviveLabel.AddComponent<TextMeshProUGUI>();
            reviveTmp.raycastTarget = false;
            reviveTmp.maskable = false;
            reviveTmp.text = "REVIVE";
            reviveTmp.alignment = TextAlignmentOptions.Center;
            reviveTmp.enableAutoSizing = true;
            reviveTmp.fontSizeMin = 16f;
            reviveTmp.fontSizeMax = 64f;
            reviveTmp.fontStyle = FontStyles.Bold;
            reviveTmp.color = Color.white;
            var reviveLabelRt = reviveLabel.GetComponent<RectTransform>();
            reviveLabelRt.anchorMin = new Vector2(0f, 0.4f);
            reviveLabelRt.anchorMax = Vector2.one;
            reviveLabelRt.sizeDelta = Vector2.zero;

            // Currency icon — bottom-left of button
            var reviveIconObj = CreateUIChild(reviveObj, "ui_image_popup_bomb_revive_icon_value");
            var reviveIconImg = reviveIconObj.AddComponent<Image>();
            reviveIconImg.raycastTarget = false;
            reviveIconImg.maskable = false;
            reviveIconImg.preserveAspect = true;
            reviveIconImg.enabled = false;
            var reviveIconRt = reviveIconObj.GetComponent<RectTransform>();
            reviveIconRt.anchorMin = new Vector2(0.05f, 0f);
            reviveIconRt.anchorMax = new Vector2(0.5f, 0.4f);
            reviveIconRt.pivot = new Vector2(0.5f, 0.5f);
            reviveIconRt.sizeDelta = Vector2.zero;

            // Cost text — bottom-right of button, next to icon
            var reviveCostObj = CreateUIChild(reviveObj, "ui_text_popup_bomb_revive_cost_value");
            var reviveCostTmp = reviveCostObj.AddComponent<TextMeshProUGUI>();
            reviveCostTmp.raycastTarget = false;
            reviveCostTmp.maskable = false;
            reviveCostTmp.text = "100";
            reviveCostTmp.alignment = TextAlignmentOptions.Center;
            reviveCostTmp.enableAutoSizing = true;
            reviveCostTmp.fontSizeMin = 12f;
            reviveCostTmp.fontSizeMax = 48f;
            reviveCostTmp.fontStyle = FontStyles.Bold;
            reviveCostTmp.color = Color.white;
            var reviveCostRt = reviveCostObj.GetComponent<RectTransform>();
            reviveCostRt.anchorMin = new Vector2(0.5f, 0f);
            reviveCostRt.anchorMax = new Vector2(0.95f, 0.4f);
            reviveCostRt.sizeDelta = Vector2.zero;

            reviveObj.SetActive(false);
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[FlowPrefabBuilder] Created " + path);
        }

        private static void BuildConfirmLossPrefab()
        {
            string path = PrefabFolder + "/ui_popup_confirm_loss.prefab";

            var root = CreateUI("ui_popup_confirm_loss");
            root.AddComponent<ConfirmLossPopupView>();
            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            // Full-screen dim — rewards panel renders on top via hierarchy order
            var dimObj = CreateUIChild(root, "ui_image_popup_confirm_dim");
            var dimImg = dimObj.AddComponent<Image>();
            dimImg.raycastTarget = true;
            dimImg.maskable = false;
            dimImg.color = new Color(0f, 0f, 0f, 0.6f);
            dimObj.AddComponent<RaycastBlocker>();
            var dimRt = dimObj.GetComponent<RectTransform>();
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.sizeDelta = Vector2.zero;

            // Compact dialog — centered in space right of rewards
            var animObj = CreateUIChild(root, "ui_anim_popup_confirm");
            var animRt = animObj.GetComponent<RectTransform>();
            animRt.anchorMin = new Vector2(0.3f, 0.25f);
            animRt.anchorMax = new Vector2(0.9f, 0.75f);
            animRt.sizeDelta = Vector2.zero;

            var frameObj = CreateUIChild(animObj, "ui_image_popup_confirm_frame");
            var frameImg = frameObj.AddComponent<Image>();
            frameImg.raycastTarget = false;
            frameImg.maskable = false;
            frameImg.type = Image.Type.Sliced;
            frameImg.color = new Color(0.18f, 0.12f, 0.12f, 0.97f);
            var frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            // Warning message
            var msgObj = CreateUIChild(animObj, "ui_text_popup_confirm_message");
            var msgTmp = msgObj.AddComponent<TextMeshProUGUI>();
            msgTmp.raycastTarget = false;
            msgTmp.maskable = false;
            msgTmp.text = "Are you sure?\nYou will lose ALL collected rewards!";
            msgTmp.alignment = TextAlignmentOptions.Center;
            msgTmp.enableAutoSizing = true;
            msgTmp.fontSizeMin = 18f;
            msgTmp.fontSizeMax = 72f;
            msgTmp.fontStyle = FontStyles.Bold;
            msgTmp.color = Color.white;
            var msgRt = msgObj.GetComponent<RectTransform>();
            msgRt.anchorMin = new Vector2(0.08f, 0.45f);
            msgRt.anchorMax = new Vector2(0.92f, 0.9f);
            msgRt.sizeDelta = Vector2.zero;

            // "Lose the rewards" button — red/danger
            var loseObj = CreateUIChild(animObj, "ui_button_popup_confirm_lose");
            var loseImg = loseObj.AddComponent<Image>();
            loseImg.raycastTarget = true;
            loseImg.maskable = false;
            loseImg.type = Image.Type.Sliced;
            loseImg.sprite = LoadButtonSprite();
            loseImg.color = new Color(0.7f, 0.15f, 0.15f, 1f);
            var loseBtn = loseObj.AddComponent<Button>();
            loseBtn.targetGraphic = loseImg;
            var loseRt = loseObj.GetComponent<RectTransform>();
            loseRt.anchorMin = new Vector2(0.05f, 0.08f);
            loseRt.anchorMax = new Vector2(0.48f, 0.35f);
            loseRt.sizeDelta = Vector2.zero;

            var loseLabelObj = CreateUIChild(loseObj, "ui_text_popup_confirm_lose_label");
            var loseTmp = loseLabelObj.AddComponent<TextMeshProUGUI>();
            loseTmp.raycastTarget = false;
            loseTmp.maskable = false;
            loseTmp.text = "LOSE REWARDS";
            loseTmp.alignment = TextAlignmentOptions.Center;
            loseTmp.enableAutoSizing = true;
            loseTmp.fontSizeMin = 14f;
            loseTmp.fontSizeMax = 52f;
            loseTmp.fontStyle = FontStyles.Bold;
            loseTmp.color = Color.white;
            var loseLabelRt = loseLabelObj.GetComponent<RectTransform>();
            loseLabelRt.anchorMin = Vector2.zero;
            loseLabelRt.anchorMax = Vector2.one;
            loseLabelRt.sizeDelta = Vector2.zero;

            // "Go Back" button — neutral
            var backObj = CreateUIChild(animObj, "ui_button_popup_confirm_back");
            var backImg = backObj.AddComponent<Image>();
            backImg.raycastTarget = true;
            backImg.maskable = false;
            backImg.type = Image.Type.Sliced;
            backImg.sprite = LoadButtonSprite();
            backImg.color = new Color(0.3f, 0.3f, 0.35f, 1f);
            var backBtn = backObj.AddComponent<Button>();
            backBtn.targetGraphic = backImg;
            var backRt = backObj.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0.52f, 0.08f);
            backRt.anchorMax = new Vector2(0.95f, 0.35f);
            backRt.sizeDelta = Vector2.zero;

            var backLabelObj = CreateUIChild(backObj, "ui_text_popup_confirm_back_label");
            var backTmp = backLabelObj.AddComponent<TextMeshProUGUI>();
            backTmp.raycastTarget = false;
            backTmp.maskable = false;
            backTmp.text = "GO BACK";
            backTmp.alignment = TextAlignmentOptions.Center;
            backTmp.enableAutoSizing = true;
            backTmp.fontSizeMin = 14f;
            backTmp.fontSizeMax = 52f;
            backTmp.fontStyle = FontStyles.Bold;
            backTmp.color = Color.white;
            var backLabelRt = backLabelObj.GetComponent<RectTransform>();
            backLabelRt.anchorMin = Vector2.zero;
            backLabelRt.anchorMax = Vector2.one;
            backLabelRt.sizeDelta = Vector2.zero;

            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[FlowPrefabBuilder] Created " + path);
        }

        private static void BuildCashOutPopupPrefab()
        {
            string path = PrefabFolder + "/ui_popup_cashout.prefab";

            var root = CreateUI("ui_popup_cashout");
            root.AddComponent<CashOutPopupView>();
            var cg = root.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            var dimObj = CreateUIChild(root, "ui_image_popup_cashout_dim");
            var dimImg = dimObj.AddComponent<Image>();
            dimImg.raycastTarget = true;
            dimImg.maskable = false;
            dimImg.color = new Color(0f, 0f, 0f, 0.7f);
            dimObj.AddComponent<RaycastBlocker>();
            var dimRt = dimObj.GetComponent<RectTransform>();
            dimRt.anchorMin = Vector2.zero;
            dimRt.anchorMax = Vector2.one;
            dimRt.sizeDelta = Vector2.zero;

            var animObj = CreateUIChild(root, "ui_anim_popup_cashout");
            var animRt = animObj.GetComponent<RectTransform>();
            animRt.anchorMin = new Vector2(0.1f, 0.15f);
            animRt.anchorMax = new Vector2(0.9f, 0.85f);
            animRt.sizeDelta = Vector2.zero;

            var frameObj = CreateUIChild(animObj, "ui_image_popup_cashout_frame");
            var frameImg = frameObj.AddComponent<Image>();
            frameImg.raycastTarget = false;
            frameImg.maskable = false;
            frameImg.type = Image.Type.Sliced;
            frameImg.color = Color.white;
            var frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            var titleObj = CreateUIChild(animObj, "ui_text_popup_cashout_title");
            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.raycastTarget = false;
            titleTmp.maskable = false;
            titleTmp.text = "Are you sure you want to exit?\nBest rewards are at the end!";
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.enableAutoSizing = true;
            titleTmp.fontSizeMin = 16f;
            titleTmp.fontSizeMax = 72f;
            titleTmp.fontStyle = FontStyles.Bold;
            var titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 0.85f);
            titleRt.anchorMax = new Vector2(1f, 0.98f);
            titleRt.sizeDelta = Vector2.zero;

            // Collect button — right side
            var continueObj = CreateUIChild(animObj, "ui_button_popup_cashout_continue");
            var continueImg = continueObj.AddComponent<Image>();
            continueImg.raycastTarget = true;
            continueImg.maskable = false;
            continueImg.type = Image.Type.Sliced;
            continueImg.sprite = LoadButtonSprite();
            continueImg.color = new Color(0.2f, 0.7f, 0.2f, 1f);
            var continueBtn = continueObj.AddComponent<Button>();
            continueBtn.targetGraphic = continueImg;
            var continueRt = continueObj.GetComponent<RectTransform>();
            continueRt.anchorMin = new Vector2(0.52f, 0.02f);
            continueRt.anchorMax = new Vector2(0.95f, 0.12f);
            continueRt.sizeDelta = Vector2.zero;

            var continueLabelObj = CreateUIChild(continueObj, "ui_text_popup_cashout_continue_label");
            var continueTmp = continueLabelObj.AddComponent<TextMeshProUGUI>();
            continueTmp.raycastTarget = false;
            continueTmp.maskable = false;
            continueTmp.text = "COLLECT";
            continueTmp.alignment = TextAlignmentOptions.Center;
            continueTmp.enableAutoSizing = true;
            continueTmp.fontSizeMin = 16f;
            continueTmp.fontSizeMax = 64f;
            continueTmp.fontStyle = FontStyles.Bold;
            continueTmp.color = Color.white;
            var continueLabelRt = continueLabelObj.GetComponent<RectTransform>();
            continueLabelRt.anchorMin = Vector2.zero;
            continueLabelRt.anchorMax = Vector2.one;
            continueLabelRt.sizeDelta = Vector2.zero;

            // Go Back button — left side
            var backObj = CreateUIChild(animObj, "ui_button_popup_cashout_back");
            var backImg = backObj.AddComponent<Image>();
            backImg.raycastTarget = true;
            backImg.maskable = false;
            backImg.type = Image.Type.Sliced;
            backImg.sprite = LoadButtonSprite();
            backImg.color = new Color(0.3f, 0.3f, 0.35f, 1f);
            var backBtn = backObj.AddComponent<Button>();
            backBtn.targetGraphic = backImg;
            var backRt = backObj.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0.05f, 0.02f);
            backRt.anchorMax = new Vector2(0.48f, 0.12f);
            backRt.sizeDelta = Vector2.zero;

            var backLabelObj = CreateUIChild(backObj, "ui_text_popup_cashout_back_label");
            var backTmp = backLabelObj.AddComponent<TextMeshProUGUI>();
            backTmp.raycastTarget = false;
            backTmp.maskable = false;
            backTmp.text = "GO BACK";
            backTmp.alignment = TextAlignmentOptions.Center;
            backTmp.enableAutoSizing = true;
            backTmp.fontSizeMin = 16f;
            backTmp.fontSizeMax = 64f;
            backTmp.fontStyle = FontStyles.Bold;
            backTmp.color = Color.white;
            var backLabelRt = backLabelObj.GetComponent<RectTransform>();
            backLabelRt.anchorMin = Vector2.zero;
            backLabelRt.anchorMax = Vector2.one;
            backLabelRt.sizeDelta = Vector2.zero;

            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log("[FlowPrefabBuilder] Created " + path);
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
