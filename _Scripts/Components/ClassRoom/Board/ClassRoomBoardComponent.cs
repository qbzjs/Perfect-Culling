using System;
using System.Collections;
using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class ClassRoomBoardComponent : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer video;
    [SerializeField]
    private SpriteRenderer slide;
    private Dictionary<int, string> videoUrls = new Dictionary<int, string>();
    private Dictionary<string, string> videoUris = new Dictionary<string, string>();
    private Dictionary<int, string> slideUrls = new Dictionary<int, string>();
    private Dictionary<string, Sprite> slideSprites = new Dictionary<string, Sprite>();

    private JObject classResourceConfigs;
    [SerializeField] private ClassQuestionObject objectQuiz;

    private ProjectorModeState projectorState = null;
    private RecordClassRoomResources recordClassRoom = default;
    private Coroutine corPrepareVideo = null;
    private Coroutine corPrepareSlide = null;
    private bool isSetTime = false;
    private bool isVideoMode = false;
    private bool isVideoPlaying = false;
    public Action onVideoStop = null;
    private double videoLength = 0;
    private double currentSeconds = 0;
    private bool _isTeacher = false;
    private int totalVideoDownload = 0;
    private int totalImageDownload = 0;
    private int countVideoDownloaded = 0;
    private int countImageDownloaded = 0;
    private int countFileDownloaded = 0;
    private bool isDownloadVideoDone = false;
    private bool isDownloadImageDone = false;

    private PopupChangeScene _popupChangeScene = null;
    private PopupChangeScene popupChangeScene
    {
        get
        {
            if (_popupChangeScene == null)
                _popupChangeScene = PanelManager.Show<PopupChangeScene>();
            return _popupChangeScene;
        }
    }
    [SerializeField] TextMeshPro textCurrentPage;
    private void SetTotalFileDownloaded(int downloaded, int total) {
        if (popupChangeScene != null)
        {
            popupChangeScene.Init(downloaded, total);
            if ((isDownloadVideoDone && isDownloadImageDone) || (total > 2 && downloaded > total * 90 / 100))
                popupChangeScene.Hide();
        }
    }


    public void ResetDefault()
    {
        videoUris.Clear();
        if (slideSprites.Count > 0)
        {
            foreach (var item in slideSprites)
            {
                Destroy(item.Value);
            }
        }
        slideSprites.Clear();
        totalVideoDownload = 0;
        totalImageDownload = 0;
        countFileDownloaded = 0;
        isDownloadVideoDone = false;
        isDownloadImageDone = false;
        video.Stop();
        SetActive(video.gameObject, true);
        SetActive(slide.gameObject, false);
    }

    public void PrepareVideo(ProjectorModeState state)
    {
        string link_prefix = state.resource_link;
        string full_link_pri = "";
        if (!string.IsNullOrEmpty(link_prefix))
        {
            int current_page_id = state.page_id;
            full_link_pri = $"{link_prefix}{current_page_id + 1}.mp4";
            if (!videoUrls.ContainsKey(current_page_id))
                videoUrls.Add(current_page_id, full_link_pri);
            int totalVideo = state.total_page;
            totalVideoDownload = totalVideo;
            VideoManager.instance.RegisterVideo(full_link_pri, AddVideoDownloaded);
            VideoManager.instance.StartDownloadVideo();
            SetTotalFileDownloaded(countFileDownloaded, totalVideoDownload + totalImageDownload);
            string full_link = "";
            for (int i = 0; i < totalVideo; i++)
            {
                if (i == current_page_id) continue;
                full_link = $"{link_prefix}{i + 1}.mp4";
                if (!videoUrls.ContainsKey(i))
                    videoUrls.Add(i, full_link);
                VideoManager.instance.RegisterVideo(full_link, AddVideoDownloaded);
            }
            VideoManager.instance.StartDownloadVideo();
        }
        if (!string.IsNullOrEmpty(full_link_pri))
        {
            LoadVideo(full_link_pri, state.page_value, state.page_state == (sbyte)ClassRoomRemoteInteactionType.Play);
        }
    }

    private void AddVideoDownloaded(string url, string uri)
    {
        if (!videoUris.ContainsKey(url))
            videoUris.Add(url, uri);
        countVideoDownloaded++;
        if (countVideoDownloaded >= totalVideoDownload)
            isDownloadVideoDone = true;
        countFileDownloaded++;
        SetTotalFileDownloaded(countFileDownloaded, totalVideoDownload + totalImageDownload);
    }
    private int _current_page;
    public void PrepareSlide(ProjectorModeState state)
    {
        string link_prefix = state.resource_link;
        string full_link_pri = "";
        if (!string.IsNullOrEmpty(link_prefix))
        {
            int current_page_id = state.page_id;
            _current_page = current_page_id;
            full_link_pri = $"{link_prefix}{current_page_id + 1}.jpg";
            if (!slideUrls.ContainsKey(current_page_id))
                slideUrls.Add(current_page_id, full_link_pri);
            ImageManager.instance.RegisterImage(full_link_pri, AddImageDownloaded, true);
            ImageManager.instance.StartDownloadImage();
            int totalSprite = state.total_page;
            totalImageDownload = totalSprite;
            SetTotalFileDownloaded(countFileDownloaded, totalVideoDownload + totalImageDownload);
            string full_link = "";
            for (int i = 0; i < totalSprite; i++)
            {
                if (i == current_page_id) continue;
                full_link = $"{link_prefix}{i + 1}.jpg";
                if (!slideUrls.ContainsKey(i))
                    slideUrls.Add(i, full_link);
                ImageManager.instance.RegisterImage(full_link, AddImageDownloaded, true);
            }
            ImageManager.instance.StartDownloadImage();
        }
        if (!string.IsNullOrEmpty(full_link_pri) && (isVideoMode == false))
        {
            PlaySlide(full_link_pri);
        }
    }
    public void PrepareQuestion(ProjectorModeState state)
    {
        if (!string.IsNullOrEmpty(state.resource_link))
        {
            StartCoroutine(IEPrepareQuestion(state.resource_link+ "resources_config.json"));
        }
    }
    private IEnumerator IEPrepareQuestion(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url)) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                string error = www.error;
                Debug.LogError(error);
            }
            else
            {
                string result = www.downloadHandler.text;
                classResourceConfigs = JObject.Parse(result);
            }
        }
    }
    private void AddImageDownloaded(string url, Texture texture)
    {
        Sprite spr = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        if (!slideSprites.ContainsKey(url))
            slideSprites.Add(url, spr);
        countImageDownloaded++;
        if (countImageDownloaded >= totalImageDownload)
            isDownloadImageDone = true;
        countFileDownloaded++;
        SetTotalFileDownloaded(countFileDownloaded, totalVideoDownload + totalImageDownload);
    }

    public void UpdateEvents(MapSchema<ProjectorModeState> state)
    {
        state.ForEach((key, value) =>
        {
            if (value.mode.ToLower().Equals(ClassRoomMediaType.slide.ToString()))
            {
                int current_page_id = value.page_id;
                _current_page = current_page_id;
                if (slideUrls.ContainsKey(current_page_id))
                {
                    PlaySlide(slideUrls[current_page_id]);
                    DisplayQuestion(current_page_id);
                }
            }
            else if (value.mode.ToLower().Equals(ClassRoomMediaType.video.ToString()))
            {
                if (value.page_state == (sbyte)ClassRoomRemoteInteactionType.Play)
                {
                    PlayVideo(value.page_value);
                    SetVideoTime(value.page_value);
                }
                else if (value.page_state == (sbyte)ClassRoomRemoteInteactionType.Pause)
                {
                    PauseVideo();
                }

            }
        });
    }

    private void LoadVideo(string url, float seconds, bool is_play)
    {
        if (corPrepareVideo != null)
            StopCoroutine(corPrepareVideo);
        corPrepareVideo = StartCoroutine(IEPrepareVideo(url, seconds, () =>
        {
            if (isVideoMode && is_play)
            {
                PlayVideo(seconds);
                SetVideoTime(seconds);
            }
        }));
    }

    private IEnumerator IEPrepareVideo(string url, float seconds, UnityAction action)
    {
        yield return new WaitUntil(() => videoUris.ContainsKey(url));
        if (!video.isPrepared)
        {
            video.errorReceived += VideoPlayer_errorReceived;
            video.source = VideoSource.Url;
            video.url = url;
            video.Prepare();
            video.isLooping = false;
            yield return new WaitUntil(() => video.isPrepared);
            videoLength = video.length;
        }
        
        action?.Invoke();
    }
    void VideoPlayer_errorReceived(VideoPlayer source, string message)
    {
        Debug.Log("error: " + message);
    }
    private PopupClassQuestion popupClassQuestion;
    public void DisplayQuestion(int question_id)
    {
        if (classResourceConfigs["quiz"] == null) return;
        if (classResourceConfigs["quiz"][$"{question_id}"] == null) {
            if (popupClassQuestion != null && popupClassQuestion.enabled)
            {
                PanelManager.Hide<PopupClassQuestion>();
                objectQuiz.gameObject.SetActive(false);
            }
            return;
        }
        RecordQuizInteractionInfo recordQuizInteractionInfo =
        classResourceConfigs["quiz"][$"{question_id}"].ToObject<RecordQuizInteractionInfo>();
        popupClassQuestion = PanelManager.Show<PopupClassQuestion>();
        popupClassQuestion.Init(recordQuizInteractionInfo, _isTeacher);
        objectQuiz.isNewQuestion = true;
        foreach (ClassQuestionAnswerLocation classQuestionAnswerLocation in objectQuiz.listLocations)
        {
            classQuestionAnswerLocation.isCorrectAnswer = false;
            classQuestionAnswerLocation.isTeacher = _isTeacher;
            classQuestionAnswerLocation.StopCorrectEffect();
        }
        objectQuiz.listLocations[recordQuizInteractionInfo.correct_answer-1].isCorrectAnswer = true;
        objectQuiz.gameObject.SetActive(true);
    }
    public void PlaySlide(string url)
    {
        if (corPrepareSlide != null)
            StopCoroutine(corPrepareSlide);
        corPrepareSlide = StartCoroutine(IEPrepareSlide(url));
    }

    private IEnumerator IEPrepareSlide(string url)
    {
        Sprite sprite = null;
        if (!slideSprites.ContainsKey(url))
        {
            ImageManager.instance.RegisterImage(url, AddImageDownloaded, true);
            ImageManager.instance.StartDownloadImage();
            yield return new WaitUntil(() => slideSprites.ContainsKey(url));
        }
        if (textCurrentPage != null)
        {
            textCurrentPage.text = _current_page.ToString();
        }
        if (slide != null)
            slide.sprite = sprite = slideSprites[url];
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        string video_link_prefix = recordClassRoom.video.link_prefix;
        if (!string.IsNullOrEmpty(video_link_prefix))
        {
            int total = recordClassRoom.video.total;
            for (int i = 0; i < total; i++)
            {
                string full_link = $"{video_link_prefix}{i + 1}.mp4";
                VideoManager.instance.UnregisterVideo(full_link, AddVideoDownloaded);
            }
        }

        string slide_link_prefix = recordClassRoom.slide.link_prefix;
        if (!string.IsNullOrEmpty(slide_link_prefix))
        {
            int total = recordClassRoom.slide.total;
            for (int i = 0; i < total; i++)
            {
                string full_link = $"{slide_link_prefix}{i + 1}.png";
                ImageManager.instance.UnregisterImage(full_link, AddImageDownloaded);
            }
        }
        
        foreach (var item in slideSprites)
        {
            Destroy(item.Value);
        }
        slideSprites.Clear();
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    public void SwitchMedia(ClassRoomMediaType type)
    {
        isVideoMode = type == ClassRoomMediaType.video;
        SetActive(video.gameObject, isVideoMode);
        SetActive(slide.gameObject, !isVideoMode);
        if (isVideoMode == false)
        {
            PauseVideo();
        }
    }

    private void PauseVideo()
    {
        isSetTime = false;
        if (video != null)
        {
            video.Pause();
            isVideoPlaying = false;
        }
    }

    private void PlayVideo(float seconds)
    {
        if (isVideoPlaying) return;
        if (video != null)
        {
            if (!video.isPrepared)
            {
                LoadVideo(video.url, seconds, true);
                return;
            }
            video.Play();
            if (currentSeconds >= videoLength)
            {
                isSetTime = false;
                SetVideoTime(0);
            }
            isVideoPlaying = true;
        }
    }

    public void ResetVideo()
    {
        if (video == null) return;
        video.Play();
        isSetTime = false;
        SetVideoTime(0);
    }

    private void StopVideo()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Pause);
        timeCountCheckVideoEnd = 0;
        isVideoPlaying = false;
        video.Stop();
        isSetTime = false;
        SetVideoTime((float)videoLength);
        onVideoStop?.Invoke();
    }

    private void SetVideoTime(float seconds)
    {
        if (isSetTime) return;
        isSetTime = true;
        currentSeconds = seconds;
        if (video != null)
            video.time = seconds;
    }


    float timeCountCheckVideoEnd = 0;
    float timeCountCheckVideoEndMax = 2;
    private void Update()
    {
        if (isVideoMode == false || isVideoPlaying == false || _isTeacher == false) return;
        double time = video.time;
        if (currentSeconds == time)
        {
            timeCountCheckVideoEnd += Time.deltaTime;
            if (timeCountCheckVideoEnd >= timeCountCheckVideoEndMax)
            {
                StopVideo();
                return;
            }
        }
        else
            timeCountCheckVideoEnd = 0;
        currentSeconds = time;
        if (time < videoLength)
            NetworkingManager.NetSend(EventName.UPDATE_VIDEO_VALUE, time);
        else
        {
            StopVideo();
        }
    }

    public void SetTeacher(bool is_teacher)
    {
        _isTeacher = is_teacher;
    }

}
