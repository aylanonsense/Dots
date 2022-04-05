using UnityEngine;
using UnityEngine.Pool;

namespace Dots
{
	public class PrefabPool : MonoBehaviour
	{
		[SerializeField] private GameObject prefab;

		private ObjectPool<GameObject> pool;

		public T Spawn<T>()
		{
			GameObject instance = pool.Get();
			return instance.GetComponent<T>();
		}

		public void Despawn(GameObject instance)
		{
			pool.Release(instance);
		}

		private void Awake()
		{
			pool = new ObjectPool<GameObject>(CreateInstance, WithdrawInstance, DepositInstance, DestroyInstance, true, 0);
		}

		private void OnDestroy()
		{
			pool.Dispose();
		}

		private GameObject CreateInstance()
		{
			GameObject instance = Instantiate(prefab);
			PoolableObject poolable = instance.GetComponent<PoolableObject>();
			if (poolable != null)
				poolable.pool = this;
			return instance;
		}

		private void WithdrawInstance(GameObject instance) => instance.SetActive(true);

		private void DepositInstance(GameObject instance) => instance.SetActive(false);

		private void DestroyInstance(GameObject instance)
		{
			if (instance != null)
				Destroy(instance);
		}
	}
}