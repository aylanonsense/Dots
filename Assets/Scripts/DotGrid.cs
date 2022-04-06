using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dots
{
	public class DotGrid : MonoBehaviour
	{
		[SerializeField] private PrefabPool dotPool;
		public int columns = 6;
		public int rows = 6;
		public float spacing = 2.2f;

		private Dot[,] dots;

		public Dot GetDot(int column, int row)
		{
			return dots[column, row];
		}

		public bool AreDotsAdjacent(Dot dot1, Dot dot2)
		{
			return (dot1.column == dot2.column && (dot1.row == dot2.row - 1 || dot1.row == dot2.row + 1)) ||
				(dot1.row == dot2.row && (dot1.column == dot2.column - 1 || dot1.column == dot2.column + 1));
		}

		public void FillWithDots(bool fallIntoPosition = true)
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					if (dots[column, row] == null)
					{
						// We found a gap in the grid, so spawn a dot to fill it
						Dot dot = dotPool.Spawn<Dot>();
						dot.OnSpawn();
						dot.colorIndex = Random.Range(0, GameManager.I.dotColors.Length);
						AddDotToGrid(dot, column, row, fallIntoPosition);
					}
				}
			}
		}

		public void ApplyGravity()
		{
			int firstRowWithGap = -1;
			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					if (dots[column, row] == null)
					{
						if (firstRowWithGap == -1)
							firstRowWithGap = row;
						// There is a gap--we need to look up the column until we find a dot that can fall down and fill it
						for (int rowAbove = row + 1; rowAbove < rows; rowAbove++)
						{
							if (dots[column, rowAbove] != null)
							{
								// We found a dot to fill the gap
								Dot dot = dots[column, rowAbove];
								dots[column, rowAbove] = null;
								dots[column, row] = dot;
								dot.row = row;
								dot.FallToPosition(CalculateCellPosition(column, row), 0.35f + ((float) (row - firstRowWithGap)) * 0.12f);
								break;
							}
						}
					}
				}
			}
		}

		public void ClearDot(Dot dot)
		{
			RemoveDotFromGrid(dot);
			dot.ShrinkAndDespawn();
		}

		public void ClearDots(IEnumerable<Dot> dots)
		{
			foreach (Dot dot in dots)
				ClearDot(dot);
		}

		public void ClearDotsOfColor(int colorIndex)
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					Dot dot = dots[column, row];
					if (dot != null && dot.colorIndex == colorIndex)
						ClearDot(dot);
				}
			}
		}

		private void Awake()
		{
			dots = new Dot[columns, rows];
		}

		private void AddDotToGrid(Dot dot, int column, int row, bool fallIntoPosition = true)
		{
			dot.grid = this;
			dot.column = column;
			dot.row = row;
			dot.transform.SetParent(transform);
			if (fallIntoPosition)
			{
				dot.transform.position = CalculateCellPosition(column, row + 6);
				dot.FallToPosition(CalculateCellPosition(column, row), 0.4f + row * 0.08f);
			}
			else
			{
				dot.transform.position = CalculateCellPosition(column, row);
			}
			dots[column, row] = dot;
		}

		private void RemoveDotFromGrid(Dot dot)
		{
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
	}
}