using UnityEngine;

namespace Dots
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T I
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<T>();
				return instance;
			}
		}

		private static T instance;

		protected static void ClearInstance()
		{
			instance = null;
		}

		protected virtual void OnEnable()
		{
			if (instance == null)
				instance = this as T;
		}

		protected virtual void OnDisable()
		{
			if (instance == this)
				instance = null;
		}
	}
}