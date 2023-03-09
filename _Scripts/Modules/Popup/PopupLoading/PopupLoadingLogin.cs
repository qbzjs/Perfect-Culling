using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Video;

[UIPanelPrefabAttr("PopupLoadingLogin", "CanvasFPS")]
public class PopupLoadingLogin : BasePanel
{
    [SerializeField] private Slider sliderloading;
    [SerializeField] private Button btTapToSkip;
    [SerializeField] private TMP_Text txVersion;
    private VideoPlayer videoPlayer;
    [SerializeField]
    private RenderTexture renderVideo;
   private Coroutine corHide;
    private Animator _animator;
    private Animator animator
    {
        get
        {

            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }

    protected override void Awake()
    {
        videoPlayer = VideoManager.instance.videoLoading;
        if(videoPlayer != null)
        videoPlayer.targetTexture = renderVideo;
    }

    private void Start()
    {
        if (btTapToSkip != null)
        {
            btTapToSkip.onClick.AddListener(HidePanel);
            btTapToSkip.gameObject.SetActive(false);
        }
        SetTextVersion();
        RectTransform rect = GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
    }

    public void PlayVideo()
    {
    if(videoPlayer != null)
        videoPlayer.Play();
    }

    private void OnEnable()
    {
        if (corHide != null)
            StopCoroutine(corHide);
        corHide = StartCoroutine(IEHide());
    }

    private void OnDisable()
    {
        if (corHide != null)
            StopCoroutine(corHide);
    }

    private void OnDestroy()
    {
        if (corHide != null)
            StopCoroutine(corHide);
    }

    private IEnumerator IEHide()
    {
        yield return new WaitForSeconds(46);
        HidePanel();
    }
    public void EventSlider90Percent()
    {
        animator.SetTrigger("90percent");
    }
    public void EventSlider100percent()
    {
        animator.SetTrigger("100percent");
        OffSliderAndOnTap();
    }
    private void OffSliderAndOnTap()
    {
        if (sliderloading == null || btTapToSkip == null) return;
        sliderloading.gameObject.SetActive(false);
        btTapToSkip.gameObject.SetActive(true);
    }
    private void SetTextVersion()
    {
        if (txVersion!=null)
            txVersion.text = "v" + Application.version;
    }

    public override void HidePanel()
    {
    if(videoPlayer != null)
        videoPlayer.Stop();
        Ultis.SetActiveCursor(false);
        TPRLSoundManager.Instance.PlayMusic("CityBackground", true, 1, 1, 0);
        PanelManager.Hide(this, true);
    }
}
