using UnityEngine;

namespace Dots
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private DotPool dotPool;

		private void Start()
		{
			dotPool.SpawnDot();
		}
	}
}