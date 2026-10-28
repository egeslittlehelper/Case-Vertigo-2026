using NUnit.Framework;
using UnityEngine;
using VertigoCase.UI;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class ObjectPoolTests
    {
        private GameObject _prefabObj;
        private GameObject _parentObj;

        [SetUp]
        public void SetUp()
        {
            _prefabObj = new GameObject("Prefab");
            _prefabObj.AddComponent<SpriteRenderer>();
            _parentObj = new GameObject("Parent");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_parentObj);
            Object.DestroyImmediate(_prefabObj);
        }

        [Test]
        public void Prewarm_CreatesCorrectCount()
        {
            var pool = new ObjectPool<SpriteRenderer>(
                _prefabObj.GetComponent<SpriteRenderer>(), _parentObj.transform, prewarm: 3);

            Assert.AreEqual(3, pool.CountInactive);
        }

        [Test]
        public void Get_ReturnsActiveObject()
        {
            var pool = new ObjectPool<SpriteRenderer>(
                _prefabObj.GetComponent<SpriteRenderer>(), _parentObj.transform, prewarm: 1);

            var instance = pool.Get();

            Assert.IsTrue(instance.gameObject.activeSelf);
            Assert.AreEqual(0, pool.CountInactive);
        }

        [Test]
        public void Release_DeactivatesAndReturnsToPool()
        {
            var pool = new ObjectPool<SpriteRenderer>(
                _prefabObj.GetComponent<SpriteRenderer>(), _parentObj.transform, prewarm: 0);

            var instance = pool.Get();
            pool.Release(instance);

            Assert.IsFalse(instance.gameObject.activeSelf);
            Assert.AreEqual(1, pool.CountInactive);
        }

        [Test]
        public void Get_AfterRelease_ReusesSameInstance()
        {
            var pool = new ObjectPool<SpriteRenderer>(
                _prefabObj.GetComponent<SpriteRenderer>(), _parentObj.transform, prewarm: 0);

            var first = pool.Get();
            pool.Release(first);
            var second = pool.Get();

            Assert.AreSame(first, second);
        }

        [Test]
        public void Get_BeyondPrewarm_InstantiatesNew()
        {
            var pool = new ObjectPool<SpriteRenderer>(
                _prefabObj.GetComponent<SpriteRenderer>(), _parentObj.transform, prewarm: 1);

            var a = pool.Get();
            var b = pool.Get();

            Assert.AreNotSame(a, b);
        }
    }
}
