using UnityEngine;

namespace Dots
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		// Manually reset static fields since domain reloading is turned off
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		protected static void ResetStaticFields() => ClearInstance();

		[SerializeField] private DotSelection dotSelection;
		[SerializeField] private DotGrid dotGrid;

		public Color[] dotColors;

		protected override void OnEnable()
		{
			base.OnEnable();
			dotGrid.onSelectDot += OnSelectDot;
			dotGrid.onHoverDotStart += OnHoverDotStart;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			dotGrid.onSelectDot -= OnSelectDot;
			dotGrid.onHoverDotStart -= OnHoverDotStart;
		}

		private void OnSelectDot(Dot dot)
		{
			if (dotSelection.numDots == 0)
			{
				dotSelection.AddDot(dot);
				dotSelection.colorIndex = dot.colorIndex;
			}
		}

		private void OnHoverDotStart(Dot dot)
		{
			if (dotSelection.numDots > 0)
			{
				// If the player hovers over the previously-selected dot, remove the current dot from the selection
				if (dot == dotSelection.previousDot)
				{
					dotSelection.RemoveMostRecentlyAddedDot();
				}
				// Otherwise, add the dot to the selection if:
				//  1. it's the same color as the currently-selected dot
				//  2. it's orthogonally adjacent to the currently-selected dot
				//  3. a link does not already exist between these two dots
				else if (dot.colorIndex == dotSelection.currentDot.colorIndex &&
					dotGrid.AreDotsAdjacent(dot, dotSelection.currentDot) &&
					!dotSelection.ContainsLink(dotSelection.currentDot, dot))
				{
					dotSelection.AddDot(dot);
				}
			}
		}
	}
}