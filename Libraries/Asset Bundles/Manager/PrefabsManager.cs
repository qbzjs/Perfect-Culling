using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrefabsManager : TPRLSingleton<PrefabsManager>
{

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    private string GetAssetName(string path)
    {
        if (!path.Contains("/") || string.IsNullOrEmpty(path)) return path;
        string[] s = path.Split('/');
        return s[s.Length - 1];
    }

    public T GetAsset<T>(string prefab_name) where T : Object
    {
        string assetName = GetAssetName(prefab_name);
        return AssetBundleDownloader.GetAsset<T>(BundleName.PREFABS, assetName);
    }

    public T GetAssetWithComponent<T>(string prefab_name) where T : Object
    {
        string assetName = GetAssetName(prefab_name);
        return AssetBundleDownloader.GetAsset<GameObject>(BundleName.PREFABS, assetName).GetComponent<T>();
    }
    public GameObject GetChildrentByName(GameObject prefabs, string nameChild)
    {
        if (prefabs == null || string.IsNullOrEmpty(nameChild)) return null;
        if (prefabs.transform.childCount == 0) return null;
        Transform transformPrefabs = prefabs.transform;
        int prefabsChildLength = transformPrefabs.childCount;
        for (int i = 0; i < prefabsChildLength; i++)
        {
            Transform child = transformPrefabs.GetChild(i);
            if (nameChild.Equals(child.name))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    //public GameObject GetAsset(string prefab_name)
    //{
    //    string assetName = GetAssetName(prefab_name);
    //    return AssetBundleLoader.instance.GetAsset<GameObject>(BundleName.PREFABS, assetName);
    //}

    //public GameObject GetPrefabWithInstantiate(string prefab_name)
    //{
    //    string assetName = GetAssetName(prefab_name);
    //    return AssetBundleLoader.instance.GetAssetWithInstantiate<GameObject>(BundleName.PREFABS, assetName);
    //}



    //public void GetPrefab(string bundle_name, string prefab_name, UnityAction<GameObject> action)
    //{
    //    string assetName = GetAssetName(prefab_name);
    //    //if (dicPrefabs.Count == 0 || !dicPrefabs.ContainsKey(assetName))
    //    //{
    //        LoadAssetBundle.LoadPrefab(bundle_name, assetName, (pf) =>
    //        {
    //            if (pf != null)
    //            {
    //                //if (!dicPrefabs.ContainsKey(assetName))
    //                //    dicPrefabs.Add(pf.name, pf);
    //            }
    //            action?.Invoke(pf);
    //        }, () =>
    //        {
    //            Debug.LogError("Khong tim thay prefab : " + assetName);
    //        });
    //        //return;
    //    //}
    //    //action?.Invoke(dicPrefabs[assetName]);
    //}

    //public void GetUnit(string bundle_name, string prefab_name, UnityAction<UnitManagerComponent> action)
    //{
    //    string assetName = GetAssetName(prefab_name);
    //    //if (dicPrefabs.Count == 0 || !dicPrefabs.ContainsKey(assetName))
    //    //{
    //        LoadAssetBundle.LoadPrefab(bundle_name, assetName, (pf) =>
    //        {
    //            if (pf != null)
    //            {
    //                //if (!dicPrefabs.ContainsKey(assetName))
    //                //    dicPrefabs.Add(pf.name, pf);
    //            }
    //            action?.Invoke(pf.GetComponent<UnitManagerComponent>());
    //        }, () =>
    //        {
    //            Debug.LogError("Khong tim thay prefab : " + assetName);
    //        });
    //        return;
    //    //}
    //    //action?.Invoke(dicPrefabs[assetName].GetComponent<UnitManagerComponent>());
    //}
}
