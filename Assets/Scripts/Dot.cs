using System;
using UnityEngine;

namespace Dots
{
	[RequireComponent(typeof(PoolableObject))]
	public class Dot : MonoBehaviour
	{
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

		private PoolableObject poolable;

		[SerializeField] private SpriteRenderer sprite;
		private int _colorIndex;

		public void Despawn()
		{
			onDespawn?.Invoke();
			if (!poolable.ReturnToPool())
				Destroy(gameObject);
		}

		private void Awake()
		{
			poolable = GetComponent<PoolableObject>();
		}

		private void OnMouseDown() => onSelect?.Invoke();
		private void OnMouseEnter() => onHoverStart?.Invoke();
		private void OnMouseExit() => onHoverEnd?.Invoke();
	}
}