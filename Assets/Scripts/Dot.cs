using System;
using UnityEngine;

namespace Dots
{
	public class Dot : MonoBehaviour
	{
		[HideInInspector] public DotPool pool;
		[HideInInspector] public DotGrid grid;
		[HideInInspector] public int column;
		[HideInInspector] public int row;
		public int colorIndex
		{
			get => _colorIndex;
			set
			{
				_colorIndex = value;
				sprite.color = color;
			}
		}
		public Color color => GameManager.I.dotColors[_colorIndex];

		public event Action onSelect;
		public event Action onHoverStart;
		public event Action onHoverEnd;
		public event Action onDespawn;

		[SerializeField] private SpriteRenderer sprite;
		private int _colorIndex;

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