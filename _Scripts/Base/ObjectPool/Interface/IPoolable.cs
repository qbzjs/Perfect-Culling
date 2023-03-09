
public interface IPoolable
{
    IObjectPool Orgin { get; set; }
    void PrepareToUse();
    void ReturnToPool();
}