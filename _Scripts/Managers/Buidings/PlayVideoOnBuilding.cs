using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideoOnBuilding : MonoBehaviour
{

    [SerializeField] private VideoPlayer[] video_Players;
    [SerializeField] private string video_Name;

    void Start()
    {
        VideoManager.instance.RegisterVideo(video_Name, PlayVideo);
        VideoManager.instance.StartDownloadVideo();
    }

    private void PlayVideo(string url, string uri)
    {
        if (video_Players == null || video_Players.Length == 0) return;
        for (int i = 0; i < video_Players.Length; i++)
        {
            StartCoroutine(IEPrepareVideo(video_Players[i], uri));
        }
    }

    IEnumerator IEPrepareVideo(VideoPlayer video_player, string url)
    {
        video_player.errorReceived += VideoPlayer_errorReceived;
        video_player.source = VideoSource.Url;
        video_player.url = url;
        video_player.Prepare();
        yield return new WaitUntil(() => video_player.isPrepared);
        video_player.Play();
        video_player.isLooping = true;
    }
    void VideoPlayer_errorReceived(VideoPlayer source, string message)
    {
        Debug.Log("error: " + message);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        VideoManager.instance.UnregisterVideo(video_Name, PlayVideo);
    }
}
