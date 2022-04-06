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
		private Color defaultBackgroundColor;

		public Color[] dotColors;
		public Color[] backgroundColors;

		private void Start()
		{
			defaultBackgroundColor = background.color;
			dotGrid.FillWithDots();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			dotSelection.onFormLoop += OnSelectDotLoop;
			dotSelection.onUnformLoop += OnDeselectDotLoop;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			dotSelection.onFormLoop -= OnSelectDotLoop;
			dotSelection.onUnformLoop -= OnDeselectDotLoop;
		}

		private void Update()
		{
			// When the mouse button is released, deselect all dots and maybe clear some dots too
			if (!Input.GetMouseButton(0) && dotSelection.numDots > 0)
			{
				ConsiderClearingSelectedDots();
				DeselectAllDots();
			}

			// Check whether the player has clicked/hovered over anything
			if (Input.GetMouseButton(0))
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.collider != null)
				{
					if (hit.collider.gameObject.TryGetComponent<Dot>(out Dot dot))
					{
						// If the player clicks a dot, start a new selection and add the dot to it
						if (Input.GetMouseButtonDown(0))
						{
							DeselectAllDots();
							SelectDot(dot);
						}
						// If the player hovers over a dot while the mouse button is down, we either select or deselect it
						else
						{
							ConsiderSelectingOrDeselectingDot(dot);
						}
					}
					else if (hit.collider.gameObject.TryGetComponent<LineSegment>(out LineSegment lineSegment))
					{
						// If the player hovers over the previous line segment, we need to deselect the current dot
						if (lineSegment == dotSelection.previousLineSegment)
						{
							DeselectMostRecentDot();
						}
					}
				}
			}
		}

		private void SelectDot(Dot dot)
		{
			dotSelection.Push(dot);
			dotSelection.colorIndex = dot.colorIndex;
			dot.Pulse();
		}

		private void DeselectMostRecentDot()
		{
			dotSelection.Pop();
		}

		private void DeselectAllDots()
		{
			dotSelection.Clear();
		}

		private void ConsiderSelectingOrDeselectingDot(Dot dot)
		{
			// If the player hovers over the previously-selected dot, deselect it
			if (dot == dotSelection.previousDot)
				DeselectMostRecentDot();
			// Otherwise, select the dot if:
			//  1. it's the same color as the currently-selected dot
			//  2. it's orthogonally adjacent to the currently-selected dot
			//  3. a link does not already exist between these two dots
			else if (dot.colorIndex == dotSelection.currentDot.colorIndex &&
					dotGrid.AreDotsAdjacent(dot, dotSelection.currentDot) &&
					!dotSelection.ContainsLink(dotSelection.currentDot, dot))
				SelectDot(dot);
		}

		private void ConsiderClearingSelectedDots()
		{
			// We've selected enough dots to clear some
			if (dotSelection.numDots >= dotSelection.minValidSelectionLength)
			{
				// Our selection contains a loop, so clear all dots of the same color in the grid
				if (dotSelection.hasLoop)
					dotGrid.ClearDotsOfColor(dotSelection.colorIndex);
				// Just clear the selected dots
				else
					dotGrid.ClearDots(dotSelection.uniqueDots);
				// Automatically tumble dots downwards and refill the grid
				dotGrid.ApplyGravity();
				dotGrid.FillWithDots();
			}
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

		private void OnSelectDotLoop()
		{
			background.color = backgroundColors[dotSelection.colorIndex];
			PulseDotsOfColor(dotSelection.colorIndex);
		}

		private void OnDeselectDotLoop()
		{
			background.color = defaultBackgroundColor;
		}
	}
}