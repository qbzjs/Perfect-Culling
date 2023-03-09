using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class EVM : MonoBehaviour
{
    public class Response<T> { public T response; }

    private readonly static string host = "https://api.gaming.chainsafe.io/evm";

    
    public void Verify(string _message, string _signature ,Action<string> action, Action error)
    {
        WWWForm form = new WWWForm();
        form.AddField("message", _message);
        form.AddField("signature", _signature);
        string url = host + "/verify";
        
        StartCoroutine(Request(url,form, action, error));
    }

   private IEnumerator Request( string url , WWWForm form, Action<string> success, Action error)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.result != UnityWebRequest.Result.Success)
            {
                error?.Invoke();
                yield break;
            }
            Response<string> data = JsonUtility.FromJson<Response<string>>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
            success?.Invoke(data.response);
            Destroy(gameObject);
        }
    }

    }
