using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using UnityEngine.Events;
//using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableLoader : TPRLSingleton<AddressableLoader>
{
    private static bool isInitAddressableDone = false;
    public Image icon;
    public Image icons;
    private static Dictionary<string, Object> lstLoadedPrefabs = new Dictionary<string, Object>();


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        //Addressables.InitializeAsync().Completed += delegate { isInitAddressableDone = true; };
    }

    //public void Load()
    //{
    //    Debug.LogError("contain : " + keyValues.ContainsKey("button-vang-da-an"));
    //    LoadAsset<Sprite>("button-vang-da-an", (s) => { icons.sprite = s; });
    //}

    //public void Unload()
    //{
    //    Debug.LogError("contain : " + keyValues.ContainsKey("button-vang-chua-an"));
    //    UnloadAsset<Sprite>(keyValues["button-vang-chua-an"]);
    //}


    //public static T LoadAsset<T>(string key) where T : Object
    //{
    //    if (isInitAddressableDone == false)
    //    {
    //        Instance.StartCoroutine(Instance.IELoadAsset<T>(key));
    //        return null;
    //    }
    //    if (lstLoadedPrefabs.ContainsKey(key))
    //        return lstLoadedPrefabs[key] as T;
    //    T result = Addressables.LoadAsset<T>(key).Result;
    //    if (!lstLoadedPrefabs.ContainsKey(key))
    //        lstLoadedPrefabs.Add(key, result as T);
    //    return result;
    //    //action?.Invoke(result);

    //    //Addressables.LoadAssetAsync<T>(key).Completed += (s) =>
    //    //{
    //    //    T result = s.Result;
    //    //    keyValues.Add(key, result as Sprite);
    //    //    action?.Invoke(result);
    //    //};

    //}

    private IEnumerator IELoadAsset<T>(string key) where T : Object
    {
        yield return new WaitUntil(() => isInitAddressableDone);
        //LoadAsset<T>(key);
    }

    //public static void LoadAssetAsync<T>(string key, UnityAction<T> action) where T : Object
    //{
    //    if (isInitAddressableDone == false)
    //    {
    //        Instance.StartCoroutine(Instance.IELoadAssetAsync<T>(key, action));
    //        return;
    //    }
    //    if (lstLoadedPrefabs.ContainsKey(key))
    //    {
    //        action?.Invoke(lstLoadedPrefabs[key] as T);
    //        return;
    //    }
    //    Addressables.LoadAssetAsync<T>(key).Completed += (s) =>
    //    {
    //        T result = s.Result;
    //        if (!lstLoadedPrefabs.ContainsKey(key))
    //            lstLoadedPrefabs.Add(key, result as T);
    //        action?.Invoke(result);
    //    };

    //}

    private IEnumerator IELoadAssetAsync<T>(string key, UnityAction<T> action) where T : Object
    {
        yield return new WaitUntil(() => isInitAddressableDone);
        //LoadAssetAsync<T>(key, action);
    }


    public static void UnloadAsset<T>(T t) where T : Object
    {
        if (isInitAddressableDone == false)
        {
            Instance.StartCoroutine(Instance.IEUnloadAsset(t));
            return;
        }
        //Addressables.ReleaseInstance(t as GameObject);
    }

    private IEnumerator IEUnloadAsset<T>(T t) where T : Object
    {
        yield return new WaitUntil(() => isInitAddressableDone);
        UnloadAsset<T>(t);
    }
}
