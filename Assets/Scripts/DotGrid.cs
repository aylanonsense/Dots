using UnityEngine;

namespace Dots
{
	public class DotGrid : MonoBehaviour
	{
		[SerializeField] private DotPool dotPool;
		[SerializeField] private int columns = 6;
		[SerializeField] private int rows = 6;
		[SerializeField] private float spacing = 2.2f;

		private void Start()
		{
			for (int column = 0; column < columns; column++)
			{
				for (int row = 0; row < rows; row++)
				{
					Dot dot = dotPool.SpawnDot();
					dot.transform.SetParent(transform);
					dot.transform.position = GetCellPosition(column, row);
				}
			}
		}

		private Vector2 GetCellPosition(int column, int row)
		{
			return new Vector2(
				spacing * (0.5f + (float) column - ((float) columns) / 2f),
				spacing * (0.5f + (float) row - ((float) rows) / 2f));
		}
	}
}