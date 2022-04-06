using System;
using UnityEngine;

namespace Dots
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(PoolableObject))]
	public class Dot : MonoBehaviour
	{
		private static readonly int PulseHash = Animator.StringToHash("Pulse");
		private static readonly int ShrinkHash = Animator.StringToHash("Shrink");

		[HideInInspector] public DotGrid grid;
		[HideInInspector] public int column;
		[HideInInspector] public int row;
		public int colorIndex
		{
			get => _colorIndex;
			set
			{
				_colorIndex = value;
				sprite.color = GameManager.I.dotColors[_colorIndex];
				pulseSprite.color = GameManager.I.dotColors[_colorIndex];
			}
		}

		public event Action onSelect;
		public event Action onHoverStart;
		public event Action onHoverEnd;
		public event Action onDespawn;

		private Animator animator;
		private PoolableObject poolable;

		[SerializeField] private SpriteRenderer sprite;
		[SerializeField] private SpriteRenderer pulseSprite;
		private int _colorIndex;

		public void Pulse()
		{
			animator.SetTrigger(PulseHash);
		}

		public void ShrinkAndDespawn()
		{
			animator.SetTrigger(ShrinkHash);
		}

		public void Despawn()
		{
			onDespawn?.Invoke();
			if (!poolable.ReturnToPool())
				Destroy(gameObject);
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			poolable = GetComponent<PoolableObject>();
		}

		private void OnMouseDown() => onSelect?.Invoke();
		private void OnMouseEnter() => onHoverStart?.Invoke();
		private void OnMouseExit() => onHoverEnd?.Invoke();
	}
}