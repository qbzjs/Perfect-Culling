//using Google.Play.AssetDelivery;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using CodeStage.AntiCheat.Storage;

public struct BuildVersion
{
    public string webgl_version;
    public string webgl_data_version;
    public string android_version;
    public string android_data_version;
    public string ios_version;
    public string ios_data_version;
    public string windows_version;
    public string windows_data_version;
    public string osx_version;
    public string osx_data_version;
}

public class AssetBundleDownloader : TPRLSingleton<AssetBundleDownloader>
{
    private static Dictionary<string, AssetBundle> dicBundlesLoaded = new Dictionary<string, AssetBundle>();
    //private static Dictionary<string, UnityWebRequest> dicWWW = new Dictionary<string, UnityWebRequest>();
    private static string assetBundleUrl;

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    protected override void OnDestroy()
    {
        dicBundlesLoaded.Clear();
        base.OnDestroy();
    }

#if UNITY_EDITOR
    private static int isSimulateAssetBundleInEditor = -1;
    private static string kSimulateAssetBundles = "SimulateAssetBundles";
    // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
    public static bool IsSimulateAssetBundleInEditor
    {
        get
        {
            if (isSimulateAssetBundleInEditor == -1)
                isSimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

            return isSimulateAssetBundleInEditor != 0;
        }
        set
        {
            int newValue = value ? 1 : 0;
            if (newValue != isSimulateAssetBundleInEditor)
            {
                isSimulateAssetBundleInEditor = newValue;
                EditorPrefs.SetBool(kSimulateAssetBundles, value);
            }
        }
    }
#endif

    public static AsyncOperation GetScene(string bundle_name, string scene_name, bool is_additive, UnityAction action, bool is_allow_activation = false)
    {
#if UNITY_EDITOR
        if (IsSimulateAssetBundleInEditor)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundle_name, scene_name);
            if (assetPaths.Length == 0)
            {
#if UNITY_EDITOR
                //LogMe.LogError("Không có asset nào tên \"" + scene_name + "\" trong bundle " + bundle_name);
#endif
                return null;
            }

            LoadSceneParameters par = new LoadSceneParameters(is_additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single);
            AsyncOperation async = EditorSceneManager.LoadSceneAsyncInPlayMode(assetPaths[0], par);
            async.allowSceneActivation = is_allow_activation;
            async.completed += delegate { action?.Invoke(); };
            return async;
        }
        else
#endif
        {
            AssetBundle bundle = null;
            try
            {
                if (dicBundlesLoaded.TryGetValue(bundle_name, out bundle) == false)
                {
#if UNITY_EDITOR
                    //LogMe.LogError("Không có asset nào tên \"" + scene_name + "\" trong bundle " + bundle_name);
#endif
                    return null;
                }
                AsyncOperation async = SceneManager.LoadSceneAsync(scene_name, is_additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single);
                async.allowSceneActivation = is_allow_activation;
                async.completed += delegate { action?.Invoke(); };
                return async;
            }
            catch (Exception e) { }
        }
        return null;
    }

    public static T GetAsset<T>(string bundle_name, string asset_name) where T : Object
    {
#if UNITY_EDITOR
        if (IsSimulateAssetBundleInEditor)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundle_name, asset_name);
            if (assetPaths.Length == 0)
            {
#if UNITY_EDITOR
                //LogMe.LogError("Không có asset nào tên \"" + asset_name + "\" trong bundle " + bundle_name);
#endif
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            return AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
        }
        else
#endif
        {
            AssetBundle bundle = null;
            try
            {
                if (dicBundlesLoaded.TryGetValue(bundle_name, out bundle) == false)
                {
#if UNITY_EDITOR
                    //LogMe.LogError("Không có asset nào tên \"" + asset_name + "\" trong bundle " + bundle_name);
#endif
                    return null;
                }
                //bundle = dicBundlesLoaded[bundle_name];
                return bundle.LoadAsset<T>(asset_name);
            }
            catch (Exception e) { }
        }
        return null;
    }

    public static void GetAssetAsync<T>(string bundle_name, string asset_name, UnityAction<T> action) where T : Object
    {
#if UNITY_EDITOR
        if (IsSimulateAssetBundleInEditor)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundle_name, asset_name);
            if (assetPaths.Length == 0)
            {
#if UNITY_EDITOR
                //LogMe.LogError("Không có asset nào tên \"" + asset_name + "\" trong bundle " + bundle_name);
#endif
                return;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            action?.Invoke(AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]));
        }
        else
#endif
        {
            AssetBundle bundle = null;
            try
            {
                if (dicBundlesLoaded.TryGetValue(bundle_name, out bundle) == false)
                {
#if UNITY_EDITOR
                    //LogMe.LogError("Không có asset nào tên \"" + asset_name + "\" trong bundle " + bundle_name);
#endif
                    return;
                }
                //bundle = dicBundlesLoaded[bundle_name];
                AssetBundleRequest request = bundle.LoadAssetAsync<T>(asset_name);
                request.completed += delegate { action?.Invoke(request.asset as T); };
            }
            catch (Exception e) { }
        }
    }

    private static float totalProcess = 0;
    public static float getProgress()
    {
        return totalProcess;
    }

    private static bool is_load_bundle_done = false;
    public static bool isLoadBundleDone {
        set => is_load_bundle_done = value;
        get => is_load_bundle_done;
    }
    private static int totalFileDownloaded = 0;
    public static float getTotalFileDownloaded()
    {
        return totalFileDownloaded;
    }
    public static void Reset()
    {
        isLoadBundleDone = false;
        foreach (var item in dicBundlesLoaded)
        {
            item.Value.Unload(true);
        }
        dicBundlesLoaded.Clear();
        totalFileDownloaded = 0;
        totalProcess = 0;
        Destroy(Instance.gameObject);
    }

    public static void SetUrl(UnityAction<int> action)
    {
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        string url = EnvironmentConfig.linkData;
        if (environment == EnvironmentType.dev || environment == EnvironmentType.test) {
            //Instance.StartCoroutine(Instance.IEGetVersion(url, delegate {
                assetBundleUrl = $"{url}{AssetBundles.Utility.GetAssetBundleNameWithoutVersion()}/";
                DownLoadManifest(action);
            //}));
           
        }
        else {
            Instance.StartCoroutine(Instance.IEGetVersion(url, delegate {
                DownLoadManifest(action);
            }));
        }
    }


    private IEnumerator IEGetVersion(string url, UnityAction action)
    {
        string url_version = EnvironmentConfig.linkGetCurrentVersion;
        using (UnityWebRequest www = UnityWebRequest.Get(url_version))
        {
            long timestamp = Ultis.GetCurrentTimeStamp();
            www.SetRequestHeader("timestamp", $"{timestamp}");
            www.SetRequestHeader("sign", Ultis.sha256_hash(timestamp));
            //www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            //www.timeout = 30;
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                BuildVersion buildVersion = JsonUtility.FromJson<BuildVersion>(response);
                string version = GetVersionByPlatform(buildVersion);
                assetBundleUrl = $"{url}{AssetBundles.Utility.GetAssetBundleNameWithoutVersion()}_{version}/";
                //Debug.LogError(assetBundleUrl);
                action?.Invoke();
            }
        }
    }

    private string GetVersionByPlatform(BuildVersion buildVersion)
    {
        string version = "";
#if UNITY_EDITOR
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        switch (target)
        {
            case BuildTarget.Android:
                version = buildVersion.android_data_version;
                break;
            case BuildTarget.iOS:
                version = buildVersion.ios_data_version;
                break;
            case BuildTarget.WebGL:
                version = buildVersion.webgl_data_version;
                break;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                version = buildVersion.windows_data_version;
                break;
            case BuildTarget.StandaloneOSX:
                version = buildVersion.osx_data_version;
                break;
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                version = buildVersion.webgl_data_version;
                break;
        }
#else
        RuntimePlatform platform = Application.platform;
        switch (platform)
        {
            case RuntimePlatform.Android:
                version = buildVersion.android_data_version;
                break;
            case RuntimePlatform.IPhonePlayer:
                version = buildVersion.ios_data_version;
                break;
            case RuntimePlatform.WebGLPlayer:
                version = buildVersion.webgl_data_version;
                break;
            case RuntimePlatform.WindowsPlayer:
                version = buildVersion.windows_data_version;
                break;
            case RuntimePlatform.OSXPlayer:
                version = buildVersion.osx_data_version;
                break;
            default:
                version = buildVersion.webgl_data_version;
                break;
        }
#endif
        return version;
    }

    //    #if !UNITY_IOS
    //    List<PlayAssetBundleRequest> lstRequest = new List<PlayAssetBundleRequest>();
    //    private void Update()
    //    {
    //        if (lstRequest.Count <= 0) return;
    //        int length = lstRequest.Count;
    //        for (int i = 0; i < length; i++)
    //        {
    //            if (i >= length) break;
    //            if (lstRequest[i] == null) {
    //                lstRequest.RemoveAt(i);
    //                i--;
    //                length = lstRequest.Count;
    //                continue;
    //            }
    //            PlayAssetBundleRequest bundleRequest = lstRequest[i];
    //            if (bundleRequest.IsDone) {
    //                AssetBundle assetBundle = bundleRequest.AssetBundle;
    //                if (!dicBundlesLoaded.ContainsKey(assetBundle.name))
    //                    dicBundlesLoaded.Add(assetBundle.name, assetBundle);
    //                totalFileDownloaded++;
    //                lstRequest.RemoveAt(i);
    //                i--;
    //                length = lstRequest.Count;
    //            }
    //        }
    //    }

    //    public static void DownLoadByPlayAssetDelivery(UnityAction<int> action)
    //    {
    //#if UNITY_EDITOR
    //        if (IsSimulateAssetBundleInEditor)
    //        {
    //            action?.Invoke(0);
    //            return;
    //        }
    //#endif
    //        string[] bundles = { "materials", "pngs", "prefabs", "scenes", "skeleton_data", "sound", "textures" };
    //        if (bundles != null)
    //        {
    //            instance.lstRequest.Clear();
    //            int length = bundles.Length;
    //            action?.Invoke(length);
    //            totalFileDownloaded = 0;
    //            for (int i = 0; i < length; i++)
    //            {
    //                CachedAssetBundle asset = new CachedAssetBundle();
    //                asset.name = bundles[i];
    //                PlayAssetBundleRequest bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(asset.name);
    //                instance.lstRequest.Add(bundleRequest);
    //            }
    //        }
    //        //instance.StopAllCoroutines();
    //        //instance.StartCoroutine(instance.IEDownLoadByPlayAssetDelivery(action));
    //    }
    //#endif
    //private IEnumerator IEDownLoadByPlayAssetDelivery(UnityAction<int> action) {
    //    string[] bundles = { "materials", "pngs", "prefabs", "scenes", "skeleton_data", "sound", "textures" };
    //    if (bundles != null)
    //    {
    //        lstRequest.Clear();
    //        int length = bundles.Length;
    //        action?.Invoke(length);
    //        totalFileDownloaded = 0;
    //        for (int i = 0; i < length; i++)
    //        {
    //            CachedAssetBundle asset = new CachedAssetBundle();
    //            asset.name = bundles[i];
    //            //asset.hash = manifest.GetAssetBundleHash(bundles[i]);
    //            PlayAssetBundleRequest bundleRequest = PlayAssetDelivery.RetrieveAssetBundleAsync(asset.name);
    //            lstRequest.Add(bundleRequest);
    //            //while (!bundleRequest.IsDone)
    //            //{
    //            //    yield return null;
    //            //}

    //            //PlayerPrefs.SetString(asset.name, asset.hash.ToString());
    //            //PlayerPrefs.Save();
    //        }
    //    }
    //}

    private static void DownLoadManifest(UnityAction<int> action)
    {
#if UNITY_EDITOR
        if (IsSimulateAssetBundleInEditor)
        {
            action?.Invoke(0);
            return;
        }
#endif
        CachedAssetBundle bundleManifest = new CachedAssetBundle();
        bundleManifest.name = AssetBundles.Utility.GetAssetBundleNameWithoutVersion();
        bundleManifest.hash = default;

        Instance.StartCoroutine(IEDownload(bundleManifest, (file) =>
        {
            AssetBundleManifest manifest = file.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            string[] bundles = manifest.GetAllAssetBundles();
            if (bundles != null)
            {
                List<CachedAssetBundle> lstDownloads = new List<CachedAssetBundle>();
                int length = bundles.Length;
                for (int i = 0; i < length; i++)
                {
                    CachedAssetBundle asset = new CachedAssetBundle();
                    asset.name = bundles[i];
                    asset.hash = manifest.GetAssetBundleHash(bundles[i]);
                    lstDownloads.Add(asset);

                }
                totalFileDownloaded = 0;
                action?.Invoke(length);
                Instance.StartCoroutine(IEDownloadBundles(lstDownloads.ToArray()));
            }
        }, false));
    }

    private static IEnumerator IEDownloadBundles(CachedAssetBundle[] bundles)
    {
        int length = bundles.Length;
        for (int i = 0; i < length; i++)
        {
            yield return Instance.StartCoroutine(IEDownload(bundles[i], null));
            yield return new WaitForEndOfFrame();
        }
    }

    private static IEnumerator IEDownload(CachedAssetBundle bundle, UnityAction<AssetBundle> action, bool is_check_hash = true)
    {
        //#if UNITY_IOS
        //        string platformName = AssetBundles.Utility.GetAssetBundleNameWithoutVersion();
        //        string uri = Path.Combine(Application.streamingAssetsPath, $"AssetBundlesOffline/{platformName}/{bundle.name}");
        //        if (uri.Contains("\\"))
        //            uri = uri.Replace('\\', '/');

        //        var fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read);
        //        var myLoadedAssetBundle = AssetBundle.LoadFromStream(fileStream);
        //        yield return new WaitForEndOfFrame();
        //        if (myLoadedAssetBundle == null)
        //        {
        //            Debug.Log("Failed to load AssetBundle!");
        //        }
        //        else
        //        {
        //            PlayerPrefs.SetString(bundle.name, bundle.hash.ToString());
        //            PlayerPrefs.Save();
        //            AssetBundle bdl = myLoadedAssetBundle;
        //            if (!dicBundlesLoaded.ContainsKey(bundle.name))
        //                dicBundlesLoaded.Add(bundle.name, bdl);
        //            totalFileDownloaded++;
        //            action?.Invoke(bdl);
        //        }
        //#else
        string url = $"{assetBundleUrl}{bundle.name}";
        UnityWebRequest uwr = null;
        //if (NetworkChecking.instance.isOfflineMode && is_check_hash == false)
        //{


        //uwr = is_check_hash ? UnityWebRequestAssetBundle.GetAssetBundle(uri, bundle.hash) : UnityWebRequestAssetBundle.GetAssetBundle(uri);
        //}
        //else
        //{
        uwr = is_check_hash ? UnityWebRequestAssetBundle.GetAssetBundle(url, bundle.hash) : UnityWebRequestAssetBundle.GetAssetBundle(url);
        //}
        using (uwr)
        {
            yield return DownloadProgress(uwr.SendWebRequest(), bundle.name);
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                ObscuredPrefs.Set<string>(bundle.name, bundle.hash.ToString());
                ObscuredPrefs.Save();
                AssetBundle bdl = DownloadHandlerAssetBundle.GetContent(uwr);
                if (bdl != null)
                {
                    if (!dicBundlesLoaded.ContainsKey(bundle.name))
                        dicBundlesLoaded.Add(bundle.name, bdl);
                    totalFileDownloaded++;
                    action?.Invoke(bdl);
                }
            }
        }
        //#endif
    }

    private static IEnumerator DownloadProgress(UnityWebRequestAsyncOperation operation, string bundle_name)
    {
        while (true)
        {
            totalProcess = operation.progress;
            if (operation.isDone && totalProcess >= 1.0f)
            {
                break;
            }
            yield return null;
        }
    }
}
