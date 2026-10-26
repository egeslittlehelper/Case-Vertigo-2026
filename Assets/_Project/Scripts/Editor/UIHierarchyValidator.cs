using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Walks the open scene and all UI prefabs, logging a clickable warning for each UI rule violation.
    /// </summary>
    public static class UIHierarchyValidator
    {
        private static int _errorCount;
        private static int _warningCount;

        [MenuItem("VertigoCase/Validate UI Hierarchy")]
        public static void Validate()
        {
            _errorCount = 0;
            _warningCount = 0;

            ValidateOpenScene();
            ValidatePrefabs();

            Debug.Log($"<b>UI Hierarchy Validation complete:</b> {_errorCount} error(s), {_warningCount} warning(s).");
        }

        private static void ValidateOpenScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            foreach (GameObject root in scene.GetRootGameObjects())
                ValidateRecursive(root, true);
        }

        private static void ValidatePrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs/UI" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                    ValidateRecursive(prefab, false);
            }
        }

        private static void ValidateRecursive(GameObject go, bool isScene)
        {
            ValidateGameObject(go, isScene);
            for (int i = 0; i < go.transform.childCount; i++)
                ValidateRecursive(go.transform.GetChild(i).gameObject, isScene);
        }

        private static void ValidateGameObject(GameObject go, bool isScene)
        {
            CheckCanvasScaler(go);
            CheckLegacyText(go);
            CheckNaming(go);
            CheckRaycastTarget(go);
            CheckMaskable(go);
            CheckAnimatorPlacement(go);
            CheckButtonOnClick(go);
            CheckSlicedSprites(go);
            CheckStretchedImages(go);
        }

        // Rule 1: Root canvas must use Expand
        private static void CheckCanvasScaler(GameObject go)
        {
            var scaler = go.GetComponent<CanvasScaler>();
            if (scaler == null) return;

            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize
                && scaler.screenMatchMode != CanvasScaler.ScreenMatchMode.Expand)
            {
                LogError(go, $"CanvasScaler screenMatchMode is {scaler.screenMatchMode}, must be Expand.");
            }
        }

        // Rule 2: No legacy Text components
        private static void CheckLegacyText(GameObject go)
        {
#pragma warning disable CS0618
            var legacyText = go.GetComponent<Text>();
#pragma warning restore CS0618
            if (legacyText != null)
                LogError(go, "Uses legacy UnityEngine.UI.Text. Use TextMeshProUGUI instead.");
        }

        // Rule 3: UI names must start with ui_
        private static void CheckNaming(GameObject go)
        {
            bool hasUIComponent = go.GetComponent<Canvas>() != null
                || go.GetComponent<Image>() != null
                || go.GetComponent<TMP_Text>() != null
                || go.GetComponent<Button>() != null
                || go.GetComponent<RectTransform>() != null;

            if (!hasUIComponent) return;

            // Skip root-level scene objects that aren't UI (EventSystem, cameras, etc.)
            if (go.transform.parent == null && go.GetComponent<Canvas>() == null)
                return;

            if (!go.name.StartsWith("ui_"))
                LogWarning(go, $"Name '{go.name}' does not start with 'ui_'.");
        }

        // Rule 4: Raycast target on without a Selectable
        private static void CheckRaycastTarget(GameObject go)
        {
            if (go.GetComponent<RaycastBlocker>() != null) return;
            if (go.GetComponent<Selectable>() != null) return;

            var image = go.GetComponent<Image>();
            if (image != null && image.raycastTarget)
                LogWarning(go, "Image has raycastTarget on but no Selectable. Turn it off or add RaycastBlocker.");

            var tmp = go.GetComponent<TMP_Text>();
            if (tmp != null && tmp.raycastTarget)
                LogWarning(go, "TMP_Text has raycastTarget on but no Selectable. Turn it off.");
        }

        // Rule 5: Maskable on without a Mask parent
        private static void CheckMaskable(GameObject go)
        {
            var image = go.GetComponent<Image>();
            if (image != null && image.maskable && !HasMaskParent(go.transform))
                LogWarning(go, "Image has maskable on but no Mask/RectMask2D in parents. Turn it off.");

            var tmp = go.GetComponent<TMP_Text>();
            if (tmp != null && tmp.maskable && !HasMaskParent(go.transform))
                LogWarning(go, "TMP_Text has maskable on but no Mask/RectMask2D in parents. Turn it off.");
        }

        private static bool HasMaskParent(Transform t)
        {
            Transform current = t.parent;
            while (current != null)
            {
                if (current.GetComponent<Mask>() != null || current.GetComponent<RectMask2D>() != null)
                    return true;
                current = current.parent;
            }
            return false;
        }

        // Rule 6: Animator on prefab root or on a view script's GameObject
        private static void CheckAnimatorPlacement(GameObject go)
        {
            var animator = go.GetComponent<Animator>();
            if (animator == null) return;

            if (go.transform.parent == null || go.GetComponent<UIViewBase>() != null)
                LogError(go, "Animator is on a root/view transform. Move it to a 'ui_anim_*' child.");
        }

        // Rule 7: No inspector OnClick events
        private static void CheckButtonOnClick(GameObject go)
        {
            var button = go.GetComponent<Button>();
            if (button != null && button.onClick.GetPersistentEventCount() > 0)
                LogError(go, "Button has inspector OnClick events. Wire listeners in code instead.");
        }

        // Rule 8: Sprite with borders should be Sliced
        private static void CheckSlicedSprites(GameObject go)
        {
            var image = go.GetComponent<Image>();
            if (image == null || image.sprite == null) return;

            Vector4 border = image.sprite.border;
            bool hasBorder = border.x > 0 || border.y > 0 || border.z > 0 || border.w > 0;

            if (hasBorder && image.type != Image.Type.Sliced)
                LogWarning(go, $"Sprite '{image.sprite.name}' has 9-slice borders but Image type is {image.type}, not Sliced.");
        }

        // Rule 9: Simple image with wrong aspect ratio (stretched)
        private static void CheckStretchedImages(GameObject go)
        {
            var image = go.GetComponent<Image>();
            if (image == null || image.sprite == null) return;
            if (image.type != Image.Type.Simple) return;
            if (image.preserveAspect) return;

            var rect = image.rectTransform.rect;
            if (rect.width <= 0 || rect.height <= 0) return;

            float spriteAspect = image.sprite.rect.width / image.sprite.rect.height;
            float rectAspect = rect.width / rect.height;

            if (spriteAspect > 0 && Mathf.Abs(rectAspect - spriteAspect) / spriteAspect > 0.05f)
                LogWarning(go, $"Image may be stretched. Sprite aspect {spriteAspect:F2} vs rect aspect {rectAspect:F2}. Use preserveAspect or Sliced.");
        }

        private static void LogError(GameObject go, string message)
        {
            _errorCount++;
            Debug.LogError($"[UI] {go.name}: {message}", go);
        }

        private static void LogWarning(GameObject go, string message)
        {
            _warningCount++;
            Debug.LogWarning($"[UI] {go.name}: {message}", go);
        }
    }
}
