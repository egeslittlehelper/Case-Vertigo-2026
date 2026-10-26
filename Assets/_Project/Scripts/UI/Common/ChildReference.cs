using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Editor-time helper that finds a child by exact name — used only from OnValidate, never at runtime.
    /// </summary>
    public static class ChildReference
    {
        public static T FindChild<T>(Transform root, string exactName) where T : Component
        {
            if (root == null) return null;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name == exactName)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        return component;
                }

                T found = FindChild<T>(child, exactName);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
