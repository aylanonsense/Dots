using UnityEngine;

namespace Dots
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private DotGrid dotGrid;

		private void OnEnable()
		{
			dotGrid.onSelectDot += OnSelectDot;
		}

		private void OnDisable()
		{
			dotGrid.onSelectDot -= OnSelectDot;
		}

		private void OnSelectDot(Dot dot)
		{
			Debug.Log($"Selected dot <{dot.column},{dot.row}>");
			dot.Despawn();
		}
	}
}