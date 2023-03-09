
public interface IObjectPool
{
    void ReturnToPool(object instance);
}
/// <summary>
/// Generic interface with more methods for object pooling
/// </summary>
public interface IObjectPool<T> : IObjectPool where T : IPoolable
{
    T GetPrefabInstance();
    void ReturnToPool(T instance);
}