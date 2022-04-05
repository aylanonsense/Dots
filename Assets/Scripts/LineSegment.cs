using System;
using UnityEngine;

namespace Dots
{
	[RequireComponent(typeof(PoolableObject))]
	public class LineSegment : MonoBehaviour
	{
		public Vector3 startPosition
		{
			get => transform.position;
			set
			{
				// Move the start point, but preserve the end position
				Vector3 currentEndPosition = endPosition;
				transform.position = value;
				endPosition = currentEndPosition;
			}
		}
		public Vector3 endPosition
		{
			get => transform.TransformPoint(Vector3.right);
			set
			{
				// Stretch the line so that it'll reach the end position
				transform.localScale = new Vector3(
					Vector3.Distance(startPosition, value),
					transform.localScale.y,
					transform.localScale.z
				);
				// Rotate it so that it'll get to the end position
				transform.eulerAngles = new Vector3(
					0f,
					0f,
					Vector3.SignedAngle(Vector3.right, value - startPosition, Vector3.forward)
				);
			}
		}
		public int colorIndex
		{
			get => _colorIndex;
			set
			{
				_colorIndex = value;
				sprite.color = GameManager.I.dotColors[_colorIndex];
			}
		}

		public event Action onHoverStart;
		public event Action onHoverEnd;
		public event Action onDespawn;

		[SerializeField] private SpriteRenderer sprite;
		private PoolableObject poolable;
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

		private void OnMouseEnter() => onHoverStart?.Invoke();
		private void OnMouseExit() => onHoverEnd?.Invoke();
	}
}