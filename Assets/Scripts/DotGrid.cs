using System;
using UnityEngine;

namespace Dots
{
	public class DotGrid : MonoBehaviour
	{
		[SerializeField] private DotPool dotPool;
		[SerializeField] private int columns = 6;
		[SerializeField] private int rows = 6;
		[SerializeField] private float spacing = 2.2f;

		public event Action<Dot> onSelectDot;
		public event Action<Dot> onHoverDotStart;
		public event Action<Dot> onHoverDotEnd;

		private DotEntry[,] dots;

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
					AddDotToGrid(dotPool.SpawnDot(), column, row);
				}
			}
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