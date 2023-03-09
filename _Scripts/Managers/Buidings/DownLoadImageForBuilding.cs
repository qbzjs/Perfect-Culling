using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownLoadImageForBuilding : MonoBehaviour
{
    [SerializeField] private string url = "";
    [SerializeField] private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DownLoadImage());
    }

    IEnumerator DownLoadImage()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        mat.SetTexture("_BaseMap", ((DownloadHandlerTexture)www.downloadHandler).texture);
    }
}
