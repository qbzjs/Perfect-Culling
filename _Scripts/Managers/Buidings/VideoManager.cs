using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoManager : SingletonMonoDontDestroy<VideoManager>
{
    private Queue<string> lstDownload = new Queue<string>();
    public bool is_VideoReady = false;
    private Dictionary<string, string> lst_Video = new Dictionary<string, string>();
    private Dictionary<string, UnityAction<string, string>> queue_ListVideo = new Dictionary<string, UnityAction<string, string>>();
    [SerializeField] private VideoPlayer _videoLoading;
    public VideoPlayer videoLoading => _videoLoading;

    public int number_OfVideoReady = 0;
    private Action<string, string> action1;
    private bool isStartDownload = false;

    private void PrepareLoadingVideo()
    {
        if(_videoLoading != null)
            _videoLoading.Prepare();
    }

    private void Start()
    {
        lstDownload.Enqueue("https://theprl-game.s3.ap-southeast-1.amazonaws.com/video/0.mp4");
        lstDownload.Enqueue("https://theprl-game.s3.ap-southeast-1.amazonaws.com/video/2.mp4");
        lstDownload.Enqueue("https://theprl-game.s3.ap-southeast-1.amazonaws.com/video/3.mp4");
        PrepareLoadingVideo();
        if (!Directory.Exists(Application.persistentDataPath + "/Video"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Video");
        }
    }

    private void LateUpdate()
    {
        if (isStartDownload == false) return;
        if(lstDownload.Count == 0)
        {
            isStartDownload = false;
            return;
        }
        string url = lstDownload.Dequeue();
        if (string.IsNullOrEmpty(url)) return;
        int index = url.Length - url.LastIndexOf(".") - 2;
        string video_name = url.Substring(url.LastIndexOf("/") + 1, (url.Length - index - url.LastIndexOf("/") - 3));
        string type_of_video = url.Substring(url.LastIndexOf(".") + 1, url.Length - url.LastIndexOf(".") - 1);
        StartCoroutine(DownloadVideo(url, video_name, type_of_video));
    }


    public void StartDownloadVideo()
    {
        isStartDownload = true;
    }

    IEnumerator DownloadVideo(string url, string name, string type_of_video)
    {
        if (!String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(type_of_video))
        {
          
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError(www.error);
                    yield break;
                }
                string path = GetPathVideoSaved($"{name}.{type_of_video}");
                File.WriteAllBytes(path, www.downloadHandler.data);
                yield return new WaitUntil(() => www.downloadHandler.isDone);
                if (!lst_Video.ContainsKey(name))
                {
                    lst_Video.Add(name, path);
                }
                if (queue_ListVideo.ContainsKey(url))
                {
                    queue_ListVideo[url]?.Invoke(url, path);
                    queue_ListVideo.Remove(url);
                }
            }
        }
    }

    public void UnregisterAll(string name)
    {
        if (queue_ListVideo.ContainsKey(name))
        {
            queue_ListVideo.Remove(name);
        }
    }

    public void ClearQueue()
    {
        queue_ListVideo.Clear();
    }
    public void RegisterVideo(string url, UnityAction<string, string> action)
    {
        if (lst_Video.ContainsKey(url))
        {
            action?.Invoke(url, lst_Video[url]);
            return;
        }
        int index = url.LastIndexOf("/");
        string video_name = url.Substring(index + 1, url.Length - index - 1);
        string path = GetPathVideoSaved(video_name);
        if (File.Exists(path)){
            action?.Invoke(url, path);
            return;
        }
        if (!lstDownload.Contains(url))
            lstDownload.Enqueue(url);

        if (queue_ListVideo.ContainsKey(url))
        {
            queue_ListVideo[url] += action;
        }
        else
        {
            queue_ListVideo.Add(url, action);
        }
    }

    public void UnregisterVideo(string name, UnityAction<string, string> action)
    {
        if (queue_ListVideo.ContainsKey(name))
        {
            queue_ListVideo[name] -= action;
        }
    }

    private string GetPathVideoSaved(string video_name)
    {
        string path = $"{Application.persistentDataPath}/Video/{video_name}";
        return path;
    }
}
