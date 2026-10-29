using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.UI;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Adds ButtonFeedback + anim child to every Button in the scene (including inactive).
    /// </summary>
    public static class ButtonFeedbackSetup
    {
        [MenuItem("VertigoCase/Setup Button Feedback")]
        public static void Run()
        {
            var buttons = Resources.FindObjectsOfTypeAll<Button>();
            int added = 0;

            foreach (var button in buttons)
            {
                if (!IsSceneObject(button))
                    continue;

                if (button.GetComponent<ButtonFeedback>() != null)
                    continue;

                EnsureAnimChild(button);
                Undo.AddComponent<ButtonFeedback>(button.gameObject);
                added++;
            }

            Debug.Log($"[ButtonFeedbackSetup] Added ButtonFeedback to {added} buttons.");
        }

        private static void EnsureAnimChild(Button button)
        {
            foreach (Transform child in button.transform)
            {
                if (child.name.StartsWith("ui_anim_"))
                    return;
            }

            var animGo = new GameObject("ui_anim_button", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(animGo, "Create anim child");
            animGo.transform.SetParent(button.transform, false);

            var rt = animGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            int childCount = button.transform.childCount;
            for (int i = childCount - 2; i >= 0; i--)
            {
                var child = button.transform.GetChild(i);
                Undo.SetTransformParent(child, animGo.transform, "Reparent to anim child");
            }

            animGo.transform.SetAsFirstSibling();
        }

        private static bool IsSceneObject(Component c)
        {
            return c != null
                   && !EditorUtility.IsPersistent(c.gameObject)
                   && c.gameObject.scene.isLoaded;
        }
    }
}
