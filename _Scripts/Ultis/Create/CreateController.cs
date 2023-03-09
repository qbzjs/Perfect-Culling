using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class CreateController : SingletonMonoDontDestroy<CreateController>
{
    private Dictionary<string, EntityManager> lstPlayers = new Dictionary<string, EntityManager>();

    public EntityManager Hero
    {
        set
        {
            if (value == null) return;
            lstPlayers.Add(value.name, value);
        }
    }

    public EntityManager GetPlayerByName(string player_name)
    {
        return lstPlayers.ContainsKey(player_name) ? lstPlayers[player_name] : null;
    }

    public T LoadResources<T>(string path) where T : MonoBehaviour
    {
        T t = Resources.Load<T>(path);
        return t;
    }

    private void AddUnitToList<T>(string tag, T t) where T : EntityManager
    {
        switch (tag)
        {
            case "Hero":
                Hero = t;
                break;
            default:
                break;
        }
    }

    //List<EnemiesPool> lstEnemyPooler = new List<EnemiesPool>();

    //List<UnitManagerComponent> lstPrefabEnemies = new List<UnitManagerComponent>();
    //List<ObjPoolController> lstObjPool = new List<ObjPoolController>();
    //public int SameElement(UnitManagerComponent enemy)
    //{
    //    for (int i = 0; i < lstPrefabEnemies.Count; i++)
    //    {
    //        if (enemy == lstPrefabEnemies[i])
    //        {
    //            return i;
    //        }
    //    }
    //    return 9999;
    //}

    //ObjPoolController enemy_pool;
    //ObjPoolController EnemyPool(GameObject prefab, ObjPoolController objOut)
    //{
    //    try
    //    {
    //        if (prefab != null && !prefab.TryGetComponent(out objOut))
    //        {
    //            objOut = prefab.AddComponent<ObjPoolController>();
    //        }
    //    }
    //    catch (System.Exception ex) { }
    //    return objOut;
    //}


    //public T CreateUnit<T>(T prefab, UnitManagerComponent enemy, Vector3 pos, Transform parent = null, string tag = "") where T : UnitManagerComponent
    //{
    //    //T ob = Instantiate(prefab, pos, prefab.transform.rotation);
    //    int index = SameElement(enemy);
    //    GameObject ob;
    //    if (index == 9999)
    //    {
    //        lstPrefabEnemies.Add(enemy);
    //        var enemyPool = gameObject.AddComponent<EnemiesPool>();
    //        lstEnemyPooler.Add(enemyPool);
    //        ObjPoolController tempObj = EnemyPool(enemy.gameObject, enemy_pool);
    //        lstObjPool.Add(tempObj);
    //        ob = enemyPool.GetObjPool(tempObj, enemy, pos).gameObject;
    //    }
    //    else
    //    {
    //        ob = lstEnemyPooler[index].GetObjPool(EnemyPool(enemy.gameObject, enemy_pool), enemy, pos).gameObject;
    //    }
    //    if (parent != null)
    //        ob.transform.SetParent(parent);
    //    ob.name = prefab.name;
    //    //T t = ob.gameObject.AddComponent<T>();
    //    T t = ob.gameObject.GetComponent<T>();
    //    if (!string.IsNullOrEmpty(tag))
    //    {
    //        ob.tag = tag;
    //        AddUnitToList<UnitManagerComponent>(tag, t);
    //    }
    //    return t;
    //}

    public T CreateUnit<T>(T prefab, Vector3 pos, Transform parent = null, string tag = "") where T : EntityManager
    {
        T ob = Instantiate(prefab, pos, prefab.transform.rotation);
        if (parent != null)
            ob.transform.SetParent(parent);
        ob.name = prefab.name;
        //T t = ob.gameObject.AddComponent<T>();
        T t = ob.gameObject.GetComponent<T>();
        if (!string.IsNullOrEmpty(tag))
        {
            ob.tag = tag;
            AddUnitToList<EntityManager>(tag, t);
        }
        return t;
    }

    public T CreateUnitLocal<T>(T prefab, Vector3 pos, Transform parent = null, string tag = "") where T : EntityManager
    {
        T ob = Instantiate(prefab);
        if (parent != null)
            ob.transform.SetParent(parent);
        ob.transform.localPosition = pos;
        ob.name = prefab.name;
        //T t = ob.gameObject.AddComponent<T>();
        T t = ob.GetComponent<T>();
        if (!string.IsNullOrEmpty(tag))
        {
            ob.tag = tag;
            AddUnitToList<EntityManager>(tag, t);
        }
        return t;
    }

    //public T CreateObjectAddComponent<T>(GameObject prefab, Vector3 pos, Transform parent = null, string tag = "") where T : MonoBehaviour
    //{
    //    GameObject ob = Instantiate(prefab, pos, prefab.transform.rotation);
    //    if (parent != null)
    //        ob.transform.SetParent(parent);
    //    ob.name = prefab.name;
    //    T t = ob.gameObject.AddComponent<T>();
    //    return t;
    //}

    public T CreateObjectGetComponent<T>(GameObject prefab, Vector3 pos, Transform parent = null, string tag = "") where T : MonoBehaviour
    {
        GameObject ob = Instantiate(prefab, pos, prefab.transform.rotation);
        if (parent != null)
            ob.transform.SetParent(parent, false);
        ob.name = prefab.name;
        T t = ob.gameObject.GetComponent<T>();
        return t;
    }

    public GameObject CreateObject(GameObject prefab, Vector3 pos, Transform parent = null, string tag = "")
    {
        GameObject ob = Instantiate(prefab, pos, prefab.transform.rotation);
        if (parent != null)
            ob.transform.SetParent(parent, false);
        ob.name = prefab.name;
        return ob;
    }

    //public UnitManagerComponent GetUnitByTag(string tag)
    //{
    //    string unitTag = tag;
    //    UnitManagerComponent[] lst = GetLstUnit(tag);
    //    if (lst == null || lst.Length == 0) return null;
    //    int count = lst.Length;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lst[i].tag.CompareTo(unitTag) != 0) continue;
    //        return lst[i];
    //    }
    //    return null;
    //}

    //public UnitManagerComponent[] GetUnitsByTag(string[] tags)
    //{
    //    List<UnitManagerComponent> units = new List<UnitManagerComponent>();
    //    if (tags == null || tags.Length == 0) return units.ToArray();
    //    int length = tags.Length;
    //    for (int i = 0; i < length; i++)
    //    {
    //        UnitManagerComponent[] lst = GetLstUnit(tags[i]);
    //        if (lst != null)
    //            units.AddRange(lst);
    //    }
    //    return units.ToArray();
    //}

    public void DestroyOb(EntityManager ob, bool is_destroy = true)
    {
        if (ob == null) return;
        //string tag = ob.tag;
        //if (tag.CompareTo("Enemy") == 0 && lstEnemies.Contains(ob))
        //{
        //    lstEnemies.Remove(ob);
        //}
        //else if (lstOthers.Contains(ob))
        //{
        //    lstOthers.Remove(ob);
        //}
        if (is_destroy)
            Destroy(ob.gameObject);
        else
            ob.gameObject.SetActive(false);
    }

    public void Remove(EntityManager unit)
    {
        if (unit == null) return;
        //string tag = unit.tag;
        //switch (tag)
        //{
        //    case "Enemy":
        //        lstEnemies.Remove(unit);
        //        break;
        //    //case "Castle":
        //    //    lst.Add(Castle);
        //    //    break;
        //    //case "Hero":
        //    //    lst = lstHeroes;
        //    //    break;
        //    //case "Archer":
        //    //    lst = lstArchers;
        //    //    break;
        //    //case "Artillery":
        //    //    lst = lstArtilleries;
        //    //    break;
        //    //case "Soldier":
        //    //    lst = lstSoldier;
        //    //    break;
        //    //default:
        //    //    lst = Others;
        //    //    break;
        //    default:
        //        break;
        //}
    }

    //public void ClearBullets()
    //{
    //    ClearHeroBullets();
    //    ClearArtilleryBullets();
    //    ClearArcherBullets();
    //}

    //public void ClearHeroBullets()
    //{
    //    int count = lstHeroes.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstHeroes[i] == null) continue;
    //        lstHeroes[i].ClearBulletClone();
    //    }
    //}

    //public void ClearArtilleryBullets()
    //{
    //    int count = lstArtilleries.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstArtilleries[i] == null) continue;
    //        lstArtilleries[i].ClearBulletClone();
    //    }
    //}

    //public void ClearArcherBullets()
    //{
    //    int count = lstArchers.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstArchers[i] == null) continue;
    //        lstArchers[i].ClearBulletClone();
    //    }
    //}

    //public void ClearEnemies()
    //{
    //    int count = lstEnemies.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstEnemies[i] == null) continue;
    //        lstEnemies[i].objPool.ReturnToPool();
    //        //Destroy(lstEnemies[i].gameObject);
    //    }
    //    lstEnemies.Clear();
    //}

    //public void Clear()
    //{
    //    ClearEnemies();
    //    int count = lstArchers.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstArchers[i] == null) continue;
    //        Destroy(lstArchers[i].gameObject);
    //    }
    //    lstArchers.Clear();
    //    count = lstArtilleries.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstArtilleries[i] == null) continue;
    //        Destroy(lstArtilleries[i].gameObject);
    //    }
    //    lstArtilleries.Clear();
    //    count = lstHeroes.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstHeroes[i] == null) continue;
    //        Destroy(lstHeroes[i].gameObject);
    //    }
    //    lstHeroes.Clear();
    //    count = lstSoldier.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstSoldier[i] == null) continue;
    //        Destroy(lstSoldier[i].gameObject);
    //    }
    //    lstSoldier.Clear();
    //    count = lstOthers.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        if (lstOthers[i] == null) continue;
    //        Destroy(lstOthers[i].gameObject);
    //    }
    //    lstOthers.Clear();
    //}
}
