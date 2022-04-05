using UnityEngine;
using UnityEngine.Pool;

namespace Dots
{
	public class DotPool : MonoBehaviour
	{
		[SerializeField] private Dot dotPrefab;

		private ObjectPool<Dot> pool;

		public Dot SpawnDot()
		{
			return pool.Get();
		}

		public void DespawnDot(Dot dot)
		{
			pool.Release(dot);
		}

		private void Awake()
		{
			pool = new ObjectPool<Dot>(CreateDot, WithdrawDot, DepositDot, DestroyDot, true, 0);
		}

		private void OnDestroy()
		{
			pool.Dispose();
		}

		private Dot CreateDot()
		{
			Dot dot = Instantiate(dotPrefab);
			dot.name = "Dot";
			dot.pool = this;
			return dot;
		}

		private void WithdrawDot(Dot dot) => dot.gameObject.SetActive(true);

		private void DepositDot(Dot dot) => dot.gameObject.SetActive(false);

		private void DestroyDot(Dot dot)
		{
			if (dot != null && dot.gameObject != null)
				Destroy(dot.gameObject);
		}
	}
}