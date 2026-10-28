using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Generic pool for Component-derived types — avoids Instantiate/Destroy churn after warm-up.
    /// </summary>
    public sealed class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Stack<T> _inactive;

        public int CountInactive => _inactive.Count;

        public ObjectPool(T prefab, Transform parent, int prewarm = 0)
        {
            _prefab = prefab;
            _parent = parent;
            _inactive = new Stack<T>(Mathf.Max(prewarm, 4));

            for (int i = 0; i < prewarm; i++)
            {
                T instance = Object.Instantiate(_prefab, _parent);
                instance.gameObject.SetActive(false);
                _inactive.Push(instance);
            }
        }

        public T Get()
        {
            T instance;

            if (_inactive.Count > 0)
            {
                instance = _inactive.Pop();
            }
            else
            {
                instance = Object.Instantiate(_prefab, _parent);
            }

            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Release(T instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_parent, false);
            _inactive.Push(instance);
        }
    }
}
