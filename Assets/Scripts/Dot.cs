using System;
using UnityEngine;

namespace Dots
{
	public class Dot : MonoBehaviour
	{
		public DotPool pool;
		public DotGrid grid;
		public int column;
		public int row;

		public event Action onSelect;
		public event Action onHoverStart;
		public event Action onHoverEnd;
		public event Action onDespawn;

		public void Despawn()
		{
			onDespawn?.Invoke();
			// Deposit the dot back in the pool if there is one
			if (pool != null)
				pool.DespawnDot(this);
			else
				Destroy(gameObject);
		}

		private void OnMouseDown() => onSelect?.Invoke();

		private void OnMouseEnter() => onHoverStart?.Invoke();

		private void OnMouseExit() => onHoverEnd?.Invoke();
	}
}