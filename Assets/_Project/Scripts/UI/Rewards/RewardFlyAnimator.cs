using System;
using DG.Tweening;
using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Spawns pooled fly icons from a wheel slice to a reward panel entry, then invokes a callback.
    /// Lives under ui_container_fly_icons (root canvas) so positions are in screen/canvas space.
    /// </summary>
    public sealed class RewardFlyAnimator : MonoBehaviour
    {
        [SerializeField] private FlyIconView _flyPrefab;
        [SerializeField] private Transform _flyContainer;
        [SerializeField] private Canvas _rootCanvas;

        [Header("Animation")]
        [SerializeField] [Range(1, 12)] private int _iconCount = 5;
        [SerializeField] private float _flyDuration = 0.5f;
        [SerializeField] private float _staggerDelay = 0.04f;
        [SerializeField] private float _spread = 40f;

        private ObjectPool<FlyIconView> _pool;
        private Sequence _flySequence;

        public void Initialize()
        {
            _pool ??= new ObjectPool<FlyIconView>(_flyPrefab, _flyContainer, _iconCount);
        }

        public void Play(Sprite icon, RectTransform from, RectTransform to, Action onArrived)
        {
            _flySequence?.Kill();
            _flySequence = DOTween.Sequence();

            Vector3 startWorld = from.position;
            Vector3 endWorld = to.position;
            bool callbackFired = false;

            for (int i = 0; i < _iconCount; i++)
            {
                FlyIconView fly = _pool.Get();
                fly.SetIcon(icon);
                fly.RectTransform.position = startWorld;
                fly.RectTransform.localScale = Vector3.one;

                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-_spread, _spread),
                    UnityEngine.Random.Range(-_spread, _spread),
                    0f);

                Vector3 target = endWorld + randomOffset * 0.1f;

                float delay = i * _staggerDelay;

                var moveTween = fly.RectTransform
                    .DOMove(target, _flyDuration)
                    .SetEase(Ease.InQuad)
                    .SetDelay(delay);

                var scaleTween = fly.RectTransform
                    .DOScale(0.3f, _flyDuration * 0.3f)
                    .SetEase(Ease.InQuad)
                    .SetDelay(delay + _flyDuration * 0.7f);

                _flySequence.Join(moveTween);
                _flySequence.Join(scaleTween);

                float arriveTime = delay + _flyDuration;
                _flySequence.InsertCallback(arriveTime, () =>
                {
                    _pool.Release(fly);
                    if (!callbackFired)
                    {
                        callbackFired = true;
                        onArrived?.Invoke();
                    }
                });
            }

            _flySequence.SetLink(gameObject);
        }

        private void OnDisable()
        {
            _flySequence?.Kill();
            _flySequence = null;
        }
    }
}
