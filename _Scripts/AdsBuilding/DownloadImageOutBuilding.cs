using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class DownloadImageOutBuilding : MonoBehaviour
{
    [SerializeField] ObscuredString [] url;
    [SerializeField] private Renderer [] renderer;

    private void Start()
    {
        if (renderer.Length > 0 && renderer != null)
        {
            for (int i = 0; i < renderer.Length; i++)
            {
                RegisterTexture(url[i], renderer[i]);
            }
            ImageManager.instance.StartDownloadImage();
        }

    }
    private void RegisterTexture(string url,Renderer renderer)
    {
        ImageManager.instance.RegisterImage(url, (url, texture) =>
        {
            renderer.material.mainTexture = texture; 
        });
    }

    
}
