using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	internal static T Instance { get; private set; }

	protected virtual void Awake ()
	{
		if (Instance != null && Instance != this)
			Destroy(gameObject);

		Instance = this as T;

		DontDestroyOnLoad(gameObject);
	}

	internal static bool Exist
	{
		get { return Instance != null; }
	}
}