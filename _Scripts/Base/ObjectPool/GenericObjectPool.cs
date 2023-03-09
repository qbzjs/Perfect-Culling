using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> : MonoBehaviour, IObjectPool<T> where T : MonoBehaviour, IPoolable
{
    // Reference to prefab.
    [SerializeField] private T prefab;
    [SerializeField]
    private int maxItemInPool = 10;

    // References to reusable instances
    private Stack<T> reusableInstances = new Stack<T>();

    /// <summary>
    /// Returns instance of prefab.
    /// </summary>
    /// <returns>Instance of prefab.</returns>
    public T GetPrefabInstance()
    {
        T inst = null;
        // if we have object in our pool we can use them
        if (reusableInstances.Count > 0)
        {
            // get object from pool
            T ob = reusableInstances.Pop();
            if (ob != null)
            {
                inst = ob;
                // remove parent
                inst.transform.SetParent(null);
                // reset position
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localScale = Vector3.one;
                inst.transform.localEulerAngles = Vector3.one;
                // activate object
                inst.gameObject.SetActive(true);
            }
            else
            {
                Destroy(ob);
                if (prefab != null)
                    inst = Instantiate(prefab);
            }
        }
        // otherwise create new instance of prefab
        else
        {
            if (prefab != null)
                inst = Instantiate(prefab);
        }
        if (inst != null)
        {
            // set reference to pool
            inst.Orgin = this;
            // and prepare instance for use
            inst.PrepareToUse();
        }
        return inst;
    }

    public void ClearLstPool()
    {
        if (reusableInstances != null)
        {
            int count = reusableInstances.Count;
            for (int i = 0; i < count; i++)
            {
                T ob = reusableInstances.Pop();
                if (ob != null)
                    Destroy(ob.gameObject);

            }
            reusableInstances.Clear();

        }
    }

    public T PrefabPool(T value)
    {
        if (value != null && prefab != value)
        {
            prefab = value;
            ClearLstPool();
        }
        return prefab;
    }

    /// <summary>
    /// Returns instance to the pool.
    /// </summary>
    /// <param name="instance">Prefab instance.</param>
    public void ReturnToPool(T instance)
    {
        if (instance != null)
        {
            // disable object
            instance.gameObject.SetActive(false);
            // set parent as this object
            instance.transform.SetParent(transform);
            // reset position
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.one;
            // add to pool
            reusableInstances.Push(instance);
            int length = reusableInstances.Count;
            if (length > maxItemInPool)
            {
                for (int i = maxItemInPool; i < length; i++)
                {
                    T ob = reusableInstances.Pop();
                    if (ob != null)
                        Destroy(ob.gameObject);
                }
            }
        }
        else
            Destroy((Object)instance);
    }

    /// <summary>
    /// Returns instance to the pool.
    /// Additional check is this is correct type.
    /// </summary>
    /// <param name="instance">Instance.</param>
    public void ReturnToPool(object instance)
    {
        // if instance is of our generic type we can return it to our pool
        if (instance is T)
        {
            ReturnToPool(instance as T);
            return;
        }
        Destroy((Object)instance);
    }
}