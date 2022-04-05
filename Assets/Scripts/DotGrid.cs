using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dots
{
	public class DotGrid : MonoBehaviour
	{
		[SerializeField] private PrefabPool dotPool;
		[SerializeField] private int columns = 6;
		[SerializeField] private int rows = 6;
		[SerializeField] private float spacing = 2.2f;

		public event Action<Dot> onSelectDot;
		public event Action<Dot> onHoverDotStart;
		public event Action<Dot> onHoverDotEnd;

		private DotEntry[,] dots;

		public void ApplyGravity()
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					if (dots[column, row] == null)
					{
						// There is a gap--we need to look up the column until we find a dot that can fall down and fill it
						for (int rowAbove = row + 1; rowAbove < rows; rowAbove++)
						{
							if (dots[column, rowAbove] != null)
							{
								// We found a dot to fill the gap
								DotEntry entry = dots[column, rowAbove];
								dots[column, rowAbove] = null;
								dots[column, row] = entry;
								entry.dot.row = row;
								entry.dot.transform.position = CalculateCellPosition(column, row);
								break;
							}
						}
					}
				}
			}
		}

		public void FillWithDots()
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					if (dots[column, row] == null)
					{
						// We found a gap in the grid to fill
						SpawnAndAddDotToGrid(column, row);
					}
				}
			}
		}

		public bool AreDotsAdjacent(Dot dot1, Dot dot2)
		{
			return (dot1.column == dot2.column && (dot1.row == dot2.row - 1 || dot1.row == dot2.row + 1)) ||
				(dot1.row == dot2.row && (dot1.column == dot2.column - 1 || dot1.column == dot2.column + 1));
		}

		public void ClearDots(IEnumerable<Dot> dots)
		{
			foreach (Dot dot in dots)
			{
				dot.Despawn();
			}
		}

		public void ClearDotsOfColor(int colorIndex)
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					if (dots[column, row] != null && dots[column, row].dot.colorIndex == colorIndex)
					{
						dots[column, row].dot.Despawn();
					}
				}
			}
		}

		private void Awake()
		{
			dots = new DotEntry[columns, rows];
		}

		private void Start()
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					SpawnAndAddDotToGrid(column, row);
				}
			}
		}

		private Dot SpawnAndAddDotToGrid(int column, int row)
		{
			Dot dot = dotPool.Spawn<Dot>();
			dot.colorIndex = Random.Range(0, GameManager.I.dotColors.Length);
			AddDotToGrid(dot, column, row);
			return dot;
		}

		private void AddDotToGrid(Dot dot, int column, int row)
		{
			dot.grid = this;
			dot.column = column;
			dot.row = row;
			dot.transform.SetParent(transform);
			dot.transform.position = CalculateCellPosition(column, row);
			// Declare event handlers so that we can properly unbind them later
			//  (since we need to create a closure around the dot variable)
			Action onSelectHandler = () => onSelectDot?.Invoke(dot);
			Action onHoverStartHandler = () => onHoverDotStart?.Invoke(dot);
			Action onHoverEndHandler = () => onHoverDotEnd?.Invoke(dot);
			Action onDespawnHandler = () => RemoveDotFromGrid(dot);
			// Bind events
			dot.onSelect += onSelectHandler;
			dot.onHoverStart += onHoverStartHandler;
			dot.onHoverEnd += onHoverEndHandler;
			dot.onDespawn += onDespawnHandler;
			// Store the dot + event handlers in the grid
			dots[column, row] = new DotEntry
			{
				dot = dot,
				onSelectHandler = onSelectHandler,
				onHoverStartHandler = onHoverStartHandler,
				onHoverEndHandler = onHoverEndHandler,
				onDespawnHandler = onDespawnHandler
			};
		}

		private void RemoveDotFromGrid(Dot dot)
		{
			// Unbind events
			DotEntry handlers = dots[dot.column, dot.row];
			dot.onSelect -= handlers.onSelectHandler;
			dot.onHoverStart -= handlers.onHoverStartHandler;
			dot.onHoverEnd -= handlers.onHoverEndHandler;
			dot.onDespawn -= handlers.onDespawnHandler;
			// Remove the dot from the grid
			dots[dot.column, dot.row] = null;
			dot.grid = null;
			dot.column = -1;
			dot.row = -1;
		}

		private Vector2 CalculateCellPosition(int column, int row)
		{
			return new Vector2(
				spacing * (0.5f + (float) column - ((float) columns) / 2f),
				spacing * (0.5f + (float) row - ((float) rows) / 2f));
		}

		private class DotEntry
		{
			public Dot dot;
			public Action onSelectHandler;
			public Action onHoverStartHandler;
			public Action onHoverEndHandler;
			public Action onDespawnHandler;
		}
	}
}