using UnityEngine;
using UnityEngine.UI;

namespace Dots
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		// Manually reset static fields since domain reloading is turned off
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		protected static void ResetStaticFields() => ClearInstance();

		[SerializeField] private DotSelection dotSelection;
		[SerializeField] private DotGrid dotGrid;
		[SerializeField] private Image background;

		public int minSelectionLength = 2;
		public Color[] dotColors;
		public Color[] backgroundColors;

		protected override void OnEnable()
		{
			base.OnEnable();
			dotGrid.onSelectDot += OnSelectDot;
			dotGrid.onHoverDotStart += OnHoverDotStart;
			dotSelection.onHoverPreviousLineSegment += OnHoverPreviousLineSegment;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			dotGrid.onSelectDot -= OnSelectDot;
			dotGrid.onHoverDotStart -= OnHoverDotStart;
			dotSelection.onHoverPreviousLineSegment -= OnHoverPreviousLineSegment;
		}

		private void Update()
		{
			// When the mouse button is released, clear the selection and maybe despawn some dots
			if (!Input.GetMouseButton(0) && dotSelection.numDots > 0)
			{
				// We've selected enough dots to clear some
				if (dotSelection.numDots >= minSelectionLength)
					ClearSelectedDots();
				// We haven't selected enough dots to clear anything, so just deselect them
				else
					DeselectDots();
			}
		}

		private void OnSelectDot(Dot dot)
		{
			if (dotSelection.numDots == 0)
				StartNewSelection(dot);
		}

		private void OnHoverDotStart(Dot dot)
		{
			if (dotSelection.numDots > 0)
				ConsiderSelectingDot(dot);
		}

		private void OnHoverPreviousLineSegment()
		{
			DeselectCurrentDot();
		}

		private void StartNewSelection(Dot dot)
		{
			// If the player clicks a dot, start a new selection and add the dot to it
			dotSelection.Clear();
			dotSelection.AddDot(dot);
			dot.Pulse();
			dotSelection.colorIndex = dot.colorIndex;
		}

		private void ConsiderSelectingDot(Dot dot)
		{
			// If the player hovers over the previously-selected dot, remove the current dot from the selection
			if (dot == dotSelection.previousDot)
				dotSelection.RemoveMostRecentlyAddedDot();
			// Otherwise, add the dot to the selection if:
			//  1. it's the same color as the currently-selected dot
			//  2. it's orthogonally adjacent to the currently-selected dot
			//  3. a link does not already exist between these two dots
			else if (dot.colorIndex == dotSelection.currentDot.colorIndex &&
					dotGrid.AreDotsAdjacent(dot, dotSelection.currentDot) &&
					!dotSelection.ContainsLink(dotSelection.currentDot, dot))
			{
				bool didHaveLoop = dotSelection.hasLoop;
				dotSelection.AddDot(dot);
				if (!didHaveLoop && dotSelection.hasLoop)
				{
					background.color = backgroundColors[dotSelection.colorIndex];
					PulseDotsOfColor(dotSelection.colorIndex);
				}
				else
				{
					dot.Pulse();
				}
			}
		}

		private void ClearSelectedDots()
		{
			// Our selection contains a loop, so clear all dots of the same color in the grid
			if (dotSelection.hasLoop)
			{
				DespawnDotsOfColor(dotSelection.colorIndex);
			}
			// Just clear the selected dots
			else
			{
				foreach (Dot dot in dotSelection.uniqueDots)
					dot.Despawn();
			}
			background.color = Color.white;
			dotSelection.Clear();
			dotGrid.ApplyGravity();
			dotGrid.FillWithDots();
		}

		private void DeselectCurrentDot()
		{
			dotSelection.RemoveMostRecentlyAddedDot();
			if (!dotSelection.hasLoop)
				background.color = Color.white;
		}

		private void DeselectDots()
		{
			dotSelection.Clear();
			background.color = Color.white;
		}

		private void PulseDotsOfColor(int colorIndex)
		{
			for (int column = 0; column < dotGrid.columns; column++)
			{
				for (int row = 0; row < dotGrid.rows; row++)
				{
					Dot dot = dotGrid.GetDot(column, row);
					if (dot != null && dot.colorIndex == dotSelection.colorIndex)
						dot.Pulse();
				}
			}
		}

		private void DespawnDotsOfColor(int colorIndex)
		{
			for (int column = 0; column < dotGrid.columns; column++)
			{
				for (int row = 0; row < dotGrid.rows; row++)
				{
					Dot dot = dotGrid.GetDot(column, row);
					if (dot != null && dot.colorIndex == dotSelection.colorIndex)
						dot.Despawn();
				}
			}
		}
	}
}