using UnityEngine;

namespace Dots
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private DotGrid dotGrid;
		[SerializeField] private LineSegment lineSegment;

		public Color[] dotColors;

		public static GameManager I
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<GameManager>();
				return instance;
			}
		}

		private static GameManager instance;

		private void Start()
		{
			lineSegment.colorIndex = 0;
			lineSegment.startPosition = new Vector2(-8f, -5f);
			lineSegment.endPosition = new Vector2(-4.5f, -3f);
		}

		private void OnEnable()
		{
			if (instance == null)
				instance = this;
			dotGrid.onSelectDot += OnSelectDot;
		}

		private void Update()
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			lineSegment.endPosition = mousePosition;
		}

		private void OnDisable()
		{
			if (instance == this)
				instance = null;
			dotGrid.onSelectDot -= OnSelectDot;
		}

		private void OnSelectDot(Dot dot)
		{
			dot.Despawn();
			dotGrid.ApplyGravity();
			dotGrid.FillWithDots();
		}
	}
}