using DG.Tweening;
using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// Shared animation parameters for popups, buttons, and UI transitions.
    /// </summary>
    [CreateAssetMenu(menuName = "VertigoCase/UI Animation Settings")]
    public sealed class UIAnimationSettingsSO : ScriptableObject
    {
        [Header("Popup")]
        [SerializeField] [Range(0.05f, 0.5f)] private float _popupFadeDuration = 0.2f;
        [SerializeField] [Range(0.1f, 0.6f)] private float _popupScaleDuration = 0.3f;
        [SerializeField] private Ease _popupShowEase = Ease.OutBack;
        [SerializeField] private Ease _popupHideEase = Ease.InBack;

        [Header("Button Feedback")]
        [SerializeField] [Range(0.5f, 1f)] private float _buttonPressScale = 0.9f;
        [SerializeField] [Range(0.02f, 0.2f)] private float _buttonPressDuration = 0.08f;
        [SerializeField] private Ease _buttonPressEase = Ease.OutQuad;

        [Header("Fly Icons")]
        [SerializeField] [Range(0.1f, 1f)] private float _flyDuration = 0.5f;
        [SerializeField] [Range(0.01f, 0.1f)] private float _flyStagger = 0.04f;

        public float PopupFadeDuration => _popupFadeDuration;
        public float PopupScaleDuration => _popupScaleDuration;
        public Ease PopupShowEase => _popupShowEase;
        public Ease PopupHideEase => _popupHideEase;
        public float ButtonPressScale => _buttonPressScale;
        public float ButtonPressDuration => _buttonPressDuration;
        public Ease ButtonPressEase => _buttonPressEase;
        public float FlyDuration => _flyDuration;
        public float FlyStagger => _flyStagger;
    }
}
