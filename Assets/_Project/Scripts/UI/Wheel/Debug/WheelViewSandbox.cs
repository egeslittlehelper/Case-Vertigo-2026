#if UNITY_EDITOR
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Temporary test harness for visual spin checks — context-menu triggers, deleted in CP07.
    /// </summary>
    [RequireComponent(typeof(WheelView))]
    public sealed class WheelViewSandbox : MonoBehaviour
    {
        [Header("Test Data")]
        [SerializeField] private WheelConfigSO _testWheel;
        [SerializeField] private RewardCatalogSO _catalog;
        [SerializeField] [Range(1, 30)] private int _testZone = 1;

        [Header("Spin Target")]
        [SerializeField] [Range(0, 11)] private int _targetSlice = 3;

        private WheelView _wheelView;

        private void Awake()
        {
            _wheelView = GetComponent<WheelView>();
        }

        private void OnEnable()
        {
            if (_wheelView != null)
                _wheelView.SpinClicked += HandleSpinClicked;
        }

        private void OnDisable()
        {
            if (_wheelView != null)
                _wheelView.SpinClicked -= HandleSpinClicked;
        }

        private void Start()
        {
            BindTestWheel();
        }

        private void HandleSpinClicked()
        {
            SpinTo(_targetSlice);
        }

        [ContextMenu("Rebind Test Wheel")]
        private void BindTestWheel()
        {
            if (_wheelView == null || _testWheel == null) return;
            _wheelView.Bind(_testWheel, _testWheel, _testZone, null, _catalog);
            _wheelView.SetInteractable(true);
            Debug.Log($"[Sandbox] Bound {_testWheel.name} with {_testWheel.Slices.Count} slices");
        }

        [ContextMenu("Spin to target slice")]
        private void SpinToTarget()
        {
            SpinTo(_targetSlice);
        }

        [ContextMenu("Spin to slice 0")]
        private void SpinToSlice0() => SpinTo(0);

        [ContextMenu("Spin to slice 3")]
        private void SpinToSlice3() => SpinTo(3);

        [ContextMenu("Spin to last slice")]
        private void SpinToLastSlice()
        {
            if (_testWheel == null) return;
            SpinTo(_testWheel.Slices.Count - 1);
        }

        private void SpinTo(int index)
        {
            if (_wheelView == null) return;
            _wheelView.SetInteractable(false);
            _wheelView.PlaySpin(index, () =>
            {
                Debug.Log($"[Sandbox] Landed on slice {index}");
                _wheelView.SetInteractable(true);
            });
        }
    }
}
#endif
