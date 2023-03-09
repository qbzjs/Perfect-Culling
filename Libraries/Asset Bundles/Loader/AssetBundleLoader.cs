using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using TMPro;
using CodeStage.AntiCheat.Storage;

public class AssetBundleLoader : TPRLSingleton<AssetBundleLoader>
{
    [SerializeField]
    private TextMeshProUGUI txProgress;
    [SerializeField]
    private Image imgProgress;
    [SerializeField]
    private GameObject loading_bar_ob;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif
    }

    bool isDestroy = false;
    protected override void OnDestroy()
    {
        isDestroy = true;
        base.OnDestroy();
    }

    public void Reset()
    {
        AssetBundleDownloader.Reset();
        Caching.ClearCache();
    }

    public T GetAsset<T>(string bundle_name, string asset_name) where T : Object
    {
        return AssetBundleDownloader.GetAsset<T>(bundle_name, asset_name);
    }

    public void GetAssetAsync<T>(string bundle_name, string asset_name, UnityAction<T> action) where T : Object
    {
        AssetBundleDownloader.GetAssetAsync<T>(bundle_name, asset_name, action);
    }

    public T GetAssetWithInstantiate<T>(string bundle_name, string asset_name) where T : Object
    {
        return Instantiate(AssetBundleDownloader.GetAsset<T>(bundle_name, asset_name));
    }

    public void GetAssetWithInstantiateAsync<T>(string bundle_name, string asset_name, UnityAction<T> action) where T : Object
    {
        AssetBundleDownloader.GetAssetAsync<T>(bundle_name, asset_name, (ob) => { T t = Instantiate(ob); action?.Invoke(t); });
    }

    private void SetActiveLoadingBar(bool active)
    {
        if (loading_bar_ob)
            loading_bar_ob.SetActive(active);
    }

    public IEnumerator LoadBundleOnline(UnityAction action, Action downloadingCallback)
    {
        if (AssetBundleDownloader.isLoadBundleDone)
        {
            SetActiveLoadingBar(false);
            action?.Invoke();
        }
        else
        {
            SetActiveLoadingBar(true);
            yield return null;
            actionBundleLoadedDone = action;
            var go = new GameObject("AssetBundleDownloader", typeof(AssetBundleDownloader));

            AssetBundleDownloader.SetUrl((total_download) =>
            {
                downloadingCallback?.Invoke();
                StartCoroutine(IECheckingDownload(total_download));
            });
            //#if BUILD_APK || UNITY_EDITOR || UNITY_IOS
            //AssetBundleDownloader.DownLoadManifest((total_download) =>
            //{
            //    StartCoroutine(IECheckingDownload(total_download));
            //});
            //#else
            //        AssetBundleDownloader.DownLoadByPlayAssetDelivery((total_download) =>
            //        {
            //            StartCoroutine(IECheckingDownloadPlayAssetDelivery(total_download));
            //        });
            //#endif
        }
    }

    private UnityAction actionBundleLoadedDone;
    private void LoadBundleDone()
    {
        AssetBundleDownloader.isLoadBundleDone = true;
        SetActiveLoadingBar(false);
        //string audio_source_prefab = "SoundManagerWebGL";
#if !UNITY_WEBGL
        string audio_source_prefab = "SoundManager";
        Instantiate(AssetBundleDownloader.GetAsset<GameObject>(BundleName.PREFABS, audio_source_prefab));
#endif
        //ScenesManager.Instance.GetScene(BundleName.SCENES, SceneName.GameSceneURP, false, null, true);
         actionBundleLoadedDone?.Invoke();
        ObscuredPrefs.Set<int>("LoadBundleDone", 1);
        ObscuredPrefs.Save();
    }



    private IEnumerator IECheckingDownloadPlayAssetDelivery(int total_download)
    {
#if UNITY_EDITOR
        // If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
        if (AssetBundleDownloader.IsSimulateAssetBundleInEditor)
        {
            LoadBundleDone();
            yield break;
        }
#endif
        if (total_download == 0)
        {
            if (txProgress != null)
                txProgress.text = $"{String.Format("{0:0.00}", 100)}%";
            LoadBundleDone();
            yield break;
        }
        int totalDownload = total_download;
        if (txProgress != null)
            txProgress.text = string.Empty;
        while (true)
        {
            if (isDestroy) break;
            float downloaded = AssetBundleDownloader.getTotalFileDownloaded();
            string downloadPercent = string.Empty;
            //float process = AssetBundleDownloader.getProgress();
            float amount = downloaded / totalDownload;
            float totalValue = amount * 100;
            if (imgProgress != null)
                imgProgress.fillAmount = amount;
            downloadPercent = $"{downloaded}/{totalDownload} ({String.Format("{0:0.00}", totalValue)}%)";
            if (txProgress != null)
                txProgress.text = downloadPercent;
            if (downloaded >= totalDownload)
            {
                LoadBundleDone();
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator IECheckingDownload(int total_download)
    {
#if UNITY_EDITOR
        // If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
        if (AssetBundleDownloader.IsSimulateAssetBundleInEditor)
        {
            LoadBundleDone();
            yield break;
        }
#endif
        //Debug.Log("IECheckingDownload: " + total_download);
        if (total_download == 0)
        {
            if (txProgress != null)
                txProgress.text = $"{String.Format("{0:0.00}", 100)}%";
            LoadBundleDone();
            yield break;
        }
        int totalDownload = total_download;
        if (txProgress != null)
            txProgress.text = string.Empty;
        while (true)
        {
            if (isDestroy) break;
            float downloaded = AssetBundleDownloader.getTotalFileDownloaded();
            string downloadPercent = string.Empty;
            float process = AssetBundleDownloader.getProgress();
            float amount = process / totalDownload;
            float totalValue = amount * 100;
            if (imgProgress != null)
            {
                imgProgress.fillAmount = process;
            }
            downloadPercent = $"{downloaded}/{totalDownload} ({String.Format("{0:0.00}", process * 100)}%)";
            if (txProgress != null)
                txProgress.text = downloadPercent;
            if (downloaded >= totalDownload)
            {
                LoadBundleDone();
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
