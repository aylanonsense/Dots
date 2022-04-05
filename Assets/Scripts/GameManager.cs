using UnityEngine;

namespace Dots
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		// Manually reset static fields since domain reloading is turned off
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		protected static void ResetStaticFields() => ClearInstance();

		[SerializeField] private DotGrid dotGrid;
		// [SerializeField] private LineSegment lineSegment;

		public Color[] dotColors;

		protected override void OnEnable()
		{
			base.OnEnable();
			dotGrid.onSelectDot += OnSelectDot;
			dotGrid.onHoverDotStart += OnHoverDotStart;
			dotGrid.onHoverDotEnd += OnHoverDotEnd;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			dotGrid.onSelectDot -= OnSelectDot;
			dotGrid.onHoverDotStart -= OnHoverDotStart;
			dotGrid.onHoverDotEnd -= OnHoverDotEnd;
		}

		private void OnSelectDot(Dot dot) {}

		private void OnHoverDotStart(Dot dot) {}

		private void OnHoverDotEnd(Dot dot) {}

		// private void Start()
		// {
		// 	lineSegment.colorIndex = 0;
		// 	lineSegment.startPosition = new Vector2(-8f, -5f);
		// 	lineSegment.endPosition = new Vector2(-4.5f, -3f);
		// }

		// private void Update()
		// {
		// 	Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// 	lineSegment.endPosition = mousePosition;
		// }

		// private void OnSelectDot(Dot dot)
		// {
		// 	dot.Despawn();
		// 	dotGrid.ApplyGravity();
		// 	dotGrid.FillWithDots();
		// }
	}
}