using System.Collections.Generic;
using UnityEngine;
public class PoolingObject : Singleton<PoolingObject>
{
    public static Dictionary<string, List<MonoBehaviour>> dic_PoolingObject = new Dictionary<string, List<MonoBehaviour>>();

    public static void DestroyPooling<Obj>(Obj _Object) where Obj : MonoBehaviour
    {
        string Type = _Object.GetType().Name;
        if (dic_PoolingObject.ContainsKey(Type))
        {
            _Object.gameObject.SetActive(false);
            dic_PoolingObject[Type].Add(_Object);
            return;
        }
        GameObject.Destroy(_Object.gameObject);
    }

    public static Obj createPooling<Obj>(Obj _Object) where Obj : MonoBehaviour
    {
        string Type = _Object.GetType().Name;
        List<MonoBehaviour> listObjects;
        if (!dic_PoolingObject.ContainsKey(Type))
        {
            listObjects = new List<MonoBehaviour>();
            dic_PoolingObject[Type] = listObjects;
            return null;
        }
        else
        {
            listObjects = dic_PoolingObject[Type];
            if (listObjects.Count > 0)
            {
                Obj obj = (Obj)listObjects[0];
                listObjects.Remove(obj);
                obj.gameObject.SetActive(true);
                return obj;
            }
            return null;
        }
    }
}
