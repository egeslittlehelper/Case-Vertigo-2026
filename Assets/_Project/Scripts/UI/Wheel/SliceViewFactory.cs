using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Grows or shrinks the pool of slice views inside a container — no Destroy/Instantiate churn on every zone.
    /// </summary>
    public sealed class SliceViewFactory : MonoBehaviour
    {
        [SerializeField] private WheelSliceView _slicePrefab;
        [SerializeField] private Transform _container;

        public WheelSliceView[] EnsureSlices(int count)
        {
            int existing = _container.childCount;

            for (int i = existing; i < count; i++)
            {
                var slice = Instantiate(_slicePrefab, _container);
                slice.name = "ui_wheel_slice";
            }

            var result = new WheelSliceView[count];
            for (int i = 0; i < count; i++)
            {
                Transform child = _container.GetChild(i);
                child.gameObject.SetActive(true);
                result[i] = child.GetComponent<WheelSliceView>();
            }

            for (int i = count; i < _container.childCount; i++)
                _container.GetChild(i).gameObject.SetActive(false);

            return result;
        }

        private void OnValidate()
        {
            if (_container == null)
                _container = ChildReference.FindChild<Transform>(transform, "ui_container_wheel_slices");
        }
    }
}
