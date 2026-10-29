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

        [Header("Anticipation")]
        [SerializeField] [Range(0f, 30f)] [Tooltip("Degrees the wheel rotates backward before spinning.")]
        private float _anticipationAngle = 10f;

        [SerializeField] [Range(0.05f, 0.5f)] [Tooltip("Duration of the back-rotation.")]
        private float _anticipationDuration = 0.15f;

        [Header("Settle")]
        [SerializeField] [Range(0f, 15f)] [Tooltip("Degrees the wheel overshoots past the target before settling.")]
        private float _settleOvershoot = 5f;

        [SerializeField] [Range(0.1f, 1f)] [Tooltip("Duration of the settle-back wobble.")]
        private float _settleDuration = 0.4f;

        [Header("Indicator Tick")]
        [SerializeField] [Range(0f, 30f)] [Tooltip("Degrees the indicator rotates on each slice boundary tick.")]
        private float _tickRotation = 15f;

        [SerializeField] [Range(0.01f, 0.2f)] [Tooltip("Duration of one tick punch.")]
        private float _tickDuration = 0.06f;

        [Header("Idle Pulse")]
        [SerializeField] [Range(0f, 0.2f)] [Tooltip("Scale variation for the spin button idle pulse.")]
        private float _idlePulseStrength = 0.06f;

        [SerializeField] [Range(0.3f, 2f)] [Tooltip("Duration of one pulse cycle.")]
        private float _idlePulseDuration = 0.8f;

        [Header("Bomb Hit")]
        [SerializeField] [Range(0f, 50f)] [Tooltip("Shake strength in pixels.")]
        private float _bombShakeStrength = 20f;

        [SerializeField] [Range(0.1f, 1f)] [Tooltip("Shake duration.")]
        private float _bombShakeDuration = 0.5f;

        [SerializeField] [Range(5, 30)] [Tooltip("Shake vibrations count.")]
        private int _bombShakeVibrato = 15;

        [Header("Result Reveal")]
        [SerializeField] [Range(0f, 1f)] [Tooltip("Scale punch strength on winning slice.")]
        private float _highlightPunchStrength = 0.25f;

        [SerializeField] [Range(0.1f, 1f)] [Tooltip("Duration of highlight punch.")]
        private float _highlightPunchDuration = 0.35f;

        [SerializeField] [Range(0f, 1f)] [Tooltip("Alpha of non-winning slices during reveal.")]
        private float _dimAlpha = 0.4f;

        public float Duration => _duration;
        public int ExtraFullTurns => _extraFullTurns;
        public Ease SpinEase => _ease;
        public float LandingJitter => _landingJitter;
        public float ResultHoldTime => _resultHoldTime;
        public float TickPunchStrength => _tickPunchStrength;
        public float SettleOvershoot => _settleOvershoot;
        public float SettleDuration => _settleDuration;
        public float AnticipationAngle => _anticipationAngle;
        public float AnticipationDuration => _anticipationDuration;
        public float TickRotation => _tickRotation;
        public float TickDuration => _tickDuration;
        public float IdlePulseStrength => _idlePulseStrength;
        public float IdlePulseDuration => _idlePulseDuration;
        public float BombShakeStrength => _bombShakeStrength;
        public float BombShakeDuration => _bombShakeDuration;
        public int BombShakeVibrato => _bombShakeVibrato;
        public float HighlightPunchStrength => _highlightPunchStrength;
        public float HighlightPunchDuration => _highlightPunchDuration;
        public float DimAlpha => _dimAlpha;
    }
}
