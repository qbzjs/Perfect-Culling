using UnityEngine;
[DisallowMultipleComponent]
public class AutoDestroy : MonoBehaviour
{
    public float TimeToDestroy = 1.5f;
    private float timeCount = 0;
    public bool isDestroy = true;
    public bool isPool;
    ItemPool obj_pool;
    ItemPool objPool
    {
        get
        {
            if (obj_pool == null)
            {
                try
                {
                    TryGetComponent<ItemPool>(out obj_pool);
                }
                catch (System.Exception ex) { }
            }
            return obj_pool;
        }
    }

    private void Update()
    {
        if (timeCount <= TimeToDestroy)
        {
            timeCount += Time.deltaTime;
            return;
        }
        Destroy();
    }


    private void Destroy()
    {
        EntityManager unit = GetComponent<EntityManager>();
        if (!isPool)
        {
            if (unit != null)
                CreateController.instance.DestroyOb(unit);
            else if (isDestroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
        else
        {
            if (unit)
            {
                unit.ClearEffects();
                CreateController.instance.Remove(unit);
                Destroy(this);
            }
            if (objPool == null)
            {
                if (isDestroy)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
                return;
            }
            objPool.ReturnToPool();
        }
    }
}
