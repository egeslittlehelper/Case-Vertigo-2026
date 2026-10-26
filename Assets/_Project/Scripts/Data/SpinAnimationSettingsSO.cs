using DG.Tweening;
using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// Tunable spin animation parameters — designer-friendly, no code changes needed.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/Spin Animation Settings")]
    public sealed class SpinAnimationSettingsSO : ScriptableObject
    {
        [SerializeField] [Range(1f, 8f)] [Tooltip("Total spin duration in seconds.")]
        private float _duration = 3.5f;

        [SerializeField] [Range(2, 10)] [Tooltip("Extra full rotations before landing.")]
        private int _extraFullTurns = 5;

        [SerializeField] [Tooltip("Easing curve for the spin deceleration.")]
        private Ease _ease = Ease.OutQuart;

        [SerializeField] [Range(0f, 0.4f)] [Tooltip("Random offset within a slice for natural landing.")]
        private float _landingJitter = 0.2f;

        [SerializeField] [Range(0f, 2f)] [Tooltip("Pause after landing before result is applied.")]
        private float _resultHoldTime = 0.5f;

        [SerializeField] [Range(0f, 5f)] [Tooltip("Scale punch on the indicator each tick.")]
        private float _tickPunchStrength = 0.15f;

        [Header("Settle")]
        [SerializeField] [Range(0f, 15f)] [Tooltip("Degrees the wheel overshoots past the target before settling.")]
        private float _settleOvershoot = 5f;

        [SerializeField] [Range(0.1f, 1f)] [Tooltip("Duration of the settle-back wobble.")]
        private float _settleDuration = 0.4f;

        public float Duration => _duration;
        public int ExtraFullTurns => _extraFullTurns;
        public Ease SpinEase => _ease;
        public float LandingJitter => _landingJitter;
        public float ResultHoldTime => _resultHoldTime;
        public float TickPunchStrength => _tickPunchStrength;
        public float SettleOvershoot => _settleOvershoot;
        public float SettleDuration => _settleDuration;
    }
}
