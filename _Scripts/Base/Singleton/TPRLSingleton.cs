using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class TPRLSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance
    {
        get
        {
            if (instance is null)
            {
                instance = FindObjectOfType<T>();
                if (instance is null)
                {
                    GameObject singleton = new GameObject(typeof(T).ToString());
                    instance = singleton.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    private static T instance;

    protected ObscuredBool dontDestroyOnLoad;

    protected virtual void Awake()
    {
        if (instance is null)
        {
            instance = this as T;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (ReferenceEquals(instance, this)) instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        if (ReferenceEquals(instance, this))
        {
            Destroy(gameObject);
            instance = null;
        }
    }
}