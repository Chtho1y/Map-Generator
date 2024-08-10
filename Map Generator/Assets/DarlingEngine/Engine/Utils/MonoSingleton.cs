using UnityEngine;


namespace DarlingEngine.Engine
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T instance;
		private static readonly object lockObject = new object();
		private static bool OnApplicationQuitting = false;


		public static T Instance
		{
			get
			{
				if (OnApplicationQuitting)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' already destroyed on application quit. Return null.");
#endif
					return null;
				}

				if (instance == null)
				{
#if UNITY_EDITOR
					Debug.LogError($"[MonoSingleton] No instances of {typeof(T).Name} found. Ensure that SingletonInitializer is set up correctly.");
#endif
					return null;
				}
				return instance;
			}
		}

		// Called when the singleton instance needs to be created manually
		public static void InitializeInstance(T newInstance)
		{
			if (instance == null)
			{
				instance = newInstance;
				DontDestroyOnLoad(newInstance.gameObject);
			}
		}

		protected virtual void Awake()
		{
			lock (lockObject)
			{
				if (instance != null && instance != this)
				{
#if UNITY_EDITOR
					Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T).Name}' already exists! Destroying object '{gameObject.name}'.");
#endif
					Destroy(gameObject);
					return;
				}

				instance = this as T;

				if (instance.transform.root.name == instance.name) DontDestroyOnLoad(gameObject);
				OnSingletonAwake();
			}
		}

		// Called when the singleton instance is awake.
		// Override this instead of Awake in derived classes.
		protected virtual void OnSingletonAwake() { }

		public virtual void Dispose()
		{
			if (instance == this)
			{
				Destroy(this);
				instance = null;
				OnApplicationQuitting = true;
			}
		}

		protected virtual void OnApplicationQuit()
		{
			OnApplicationQuitting = true;
		}
	}


	public class SingletonInitializer
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void CreateSingleton<T>() where T : MonoSingleton<T>, new()
		{
			if (MonoSingleton<T>.Instance == null)
			{
				GameObject singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
				T instance = singletonObject.AddComponent<T>();
				Object.DontDestroyOnLoad(singletonObject);
				MonoSingleton<T>.InitializeInstance(instance);
			}
		}
	}
}