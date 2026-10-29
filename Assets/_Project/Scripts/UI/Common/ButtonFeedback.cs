using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VertigoCase.UI
{
    /// <summary>
    /// Press scale feedback on the ui_anim_* child of a button.
    /// </summary>
    public sealed class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Transform _animTarget;
        [SerializeField] [Range(0.5f, 1f)] private float _pressScale = 0.9f;
        [SerializeField] [Range(0.02f, 0.2f)] private float _duration = 0.08f;

        private Tween _tween;
        private Vector3 _originalScale = Vector3.one;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_animTarget == null) return;
            _tween?.Kill();
            _tween = _animTarget.DOScale(_originalScale * _pressScale, _duration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_animTarget == null) return;
            _tween?.Kill();
            _tween = _animTarget.DOScale(_originalScale, _duration)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }

        private void Awake()
        {
            if (_animTarget != null)
                _originalScale = _animTarget.localScale;
        }

        private void OnValidate()
        {
            if (_animTarget == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.StartsWith("ui_anim_"))
                    {
                        _animTarget = child;
                        break;
                    }
                }
            }
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
            if (_animTarget != null)
                _animTarget.localScale = _originalScale;
        }
    }
}
