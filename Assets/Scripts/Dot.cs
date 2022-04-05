using System;
using UnityEngine;

namespace Dots
{
	public class Dot : MonoBehaviour
	{
		public DotPool pool;

		public event Action onSelect;
		public event Action onHoverStart;
		public event Action onHoverEnd;

		public void Despawn()
		{
			if (pool != null)
				pool.DespawnDot(this);
			else
				Destroy(gameObject);
		}

		private void OnMouseDown()
		{
			onSelect?.Invoke();
		}

		private void OnMouseEnter()
		{
			onHoverStart?.Invoke();
		}

		private void OnMouseExit()
		{
			onHoverEnd?.Invoke();
		}
	}
}