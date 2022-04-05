using UnityEngine;

namespace Dots
{
	public class PoolableObject : MonoBehaviour
	{
		[HideInInspector] public PrefabPool pool;

		public bool ReturnToPool()
		{
			if (pool != null)
			{
				pool.Despawn(gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}