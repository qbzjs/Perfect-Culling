using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class Splash : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txVersion;
    [SerializeField]
    private PopUpNotice popupNotice;
    private bool isHaveInternet = false;

    private bool IsInternetConnected()
    {
        isHaveInternet = !(Application.internetReachability == NetworkReachability.NotReachable);
        if (!isHaveInternet)
        {
            popupNotice.OnSetTextOneButton(Constant.CONNECTION_ERROR_NAME, Constant.CONNECTION_ERROR_CONTENT, null, "OK");
            popupNotice.gameObject.SetActive(true);
        }
        return isHaveInternet;
    }

    private void Awake()
    {
        Application.runInBackground = true;
        if (txVersion)
            txVersion.text = "v" + Application.version;
        StartCoroutine(IEGetGameConfig(delegate { StartCoroutine(DownloadBundle()); }));
    }

    private IEnumerator IEGetGameConfig(UnityAction action)
    {
        while (!IsInternetConnected())
        {
            yield return new WaitForSeconds(1);
        }
        TPRLAPI.instance.GetGameConfig(action);
    }

    private IEnumerator DownloadBundle()
    {
        while (!IsInternetConnected())
        {
            yield return new WaitForSeconds(1);
        }

        float t = 0;
        bool startDownload = false;
        bool isDownloadingBundle = false;
        while (!isDownloadingBundle)
        {
            if (!startDownload)
            {
                startDownload = true;
                AssetBundleLoader.Instance.StartCoroutine(AssetBundleLoader.Instance.LoadBundleOnline(delegate { isDownloadingBundle = true; Init(); }, () => { isDownloadingBundle = true; }));
            }
            t += Time.deltaTime;
            if (t > 10 && isDownloadingBundle == false)
            {
                IsInternetConnected();
                AssetBundleLoader.Instance.Reset();
                startDownload = false;
                t = 0;
            }
            yield return null;
        }
    }

    private void Init()
    {
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.Login, false, null, true);
    }
}
