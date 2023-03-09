using UnityEngine;

public class ItemPoolManager : GenericObjectPool<ItemPool>
{

    public ItemPool GetInstance(ItemPool prefab = null)
    {
        PrefabPool(prefab);
        ItemPool pr = GetPrefabInstance();
        if (pr)
        {
            pr.transform.SetParent(transform);
            pr.transform.localScale = Vector3.one;
            pr.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        return pr;
    }
}
