using UnityEngine;
using System.Net;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Events;
public class NetworkChecking : SingletonMonoDontDestroy<NetworkChecking>
{
    //public static NetworkChecking Instance;
    //public Text txNetwork;
    public bool isOfflineMode = false;
    private bool isConnected = false;
    //[SerializeField]
    //public PopupNetwork popupNetwork ;
    bool is_destroy = false;

    //public void SetActivePopupNetwork(bool active)
    //{
    //    if(popupNetwork)
    //        popupNetwork.SetActive(active);
    //} 
    //public bool setActivePopupErrorUpdateAndPatchAvailaible 
    //{ 
    //    set 
    //    { 
    //        if(popupNetwork != null)
    //        {
    //            popupNetwork.SetAcivepopupErrorUpdate(value); 
    //            popupNetwork.SetAcivePopupPatchAvailaible(!value);
    //        }
    //    } 
    //}

    //// Use this for initialization
    //public void ClickYesPopupErrorUpdate(UnityAction action)
    //{
    //    if(popupNetwork != null)
    //        popupNetwork.SetYesBtnPopupErrorUpdate(action);
    //}
    //public void ClickYesPopupPatchAvailaible(UnityAction action)
    //{
    //    if (popupNetwork != null)
    //        popupNetwork.SetYesBtnPopupPatchAvailaible(action);
    //}
    //public void ClickNoPopupErrorUpdate(UnityAction action)
    //{
    //    if (popupNetwork != null)
    //        popupNetwork.SetNoBtnPopupErrorUpdate(action);
    //}
    //public void ClickNoPopupPatchAvailaible(UnityAction action)
    //{
    //    if (popupNetwork != null)
    //        popupNetwork.SetNoBtnPopupPatchAvailaible(action);
    //}
    public void Run(Action<int> action)
    {
        StopAllCoroutines();
        IsConnected(true, (is_connected)=>
        {
            isConnected = is_connected;
            isOfflineMode = !is_connected;
            action?.Invoke(is_connected ? 1 : 0);
        });
        IECheckingNetworkState();
    }
    private async void IECheckingNetworkState()
    {
        while (true)
        {
            await Task.Delay(15000);
            if (is_destroy) break;
            IsConnected(false, (is_connected)=>
            {
                if(isConnected != is_connected)
                {
                    isConnected = is_connected;
                    isOfflineMode = !is_connected;
                    //EventManager.TriggerEvent(is_connected ? "NetworkConnected" : "NetworkDisconnected");
                }
            });
        }
    }

    public void IsConnected(bool is_push_event = false, Action<bool> success = null)
    {

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    if (is_push_event)
        //    {
        //        Debug.LogError("Push event disconnected 1 ");
        //        EventManager.TriggerEvent("NetworkDisconnected");
        //    }
        //    error?.Invoke();
        //}
        //else
        //{

        StartCoroutine(IEGetHtmlFromUri("https://www.google.com", (is_connected)=>
        {
            if (is_push_event)
            {
                    //EventManager.TriggerEvent("NetworkConnected");
            }
            success?.Invoke(is_connected);
        }));
        //}
    }

    private IEnumerator IEGetHtmlFromUri(string url = "https://www.google.com", Action<bool> success = null)
    {
        bool isSuccess = false;
        using (UnityWebRequest google = UnityWebRequest.Get(url))
        {
            google.timeout = 10;
            yield return google.SendWebRequest();
            if (google.result == UnityWebRequest.Result.ConnectionError || google.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(google.error);
                success?.Invoke(false);
            }
            else
            {
                isSuccess = google.responseCode < 299 && (int)google.responseCode >= 200;
                if (!isSuccess)
                {
                    using (UnityWebRequest youtube = UnityWebRequest.Get("https://www.youtube.com"))
                    {
                        youtube.timeout = 10;
                        yield return youtube.SendWebRequest();
                        if (youtube.result == UnityWebRequest.Result.ConnectionError || youtube.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.LogError(youtube.error);
                        }
                        else
                        {
                            isSuccess = youtube.responseCode < 299 && (int)youtube.responseCode >= 200;
                            success?.Invoke(isSuccess);
                        }
                    }
                }
                else
                    success?.Invoke(true);
            }
        }
    }

    private void OnDestroy()
    {
        is_destroy = true;
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
}
