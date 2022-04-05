using UnityEngine;

namespace Dots
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private DotGrid dotGrid;

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

		private void OnEnable()
		{
			if (instance == null)
				instance = this;
			dotGrid.onSelectDot += OnSelectDot;
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