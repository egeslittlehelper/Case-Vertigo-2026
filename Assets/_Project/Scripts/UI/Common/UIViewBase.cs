using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Base class for all views — provides Show/Hide and the OnValidate auto-assign pattern.
    /// </summary>
    public abstract class UIViewBase : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        protected virtual void OnValidate()
        {
            if (_root == null)
                _root = gameObject;
        }

        public virtual void Show()
        {
            if (_root != null)
                _root.SetActive(true);
        }

        public virtual void Hide()
        {
            if (_root != null)
                _root.SetActive(false);
        }
    }
}
