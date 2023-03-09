using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private TMP_Text tx_MusicVolumnNumber;
    [SerializeField] private TMP_Text tx_SFXVolumnNumber;
    [SerializeField] private TMP_Text tx_VideoSFXVolumnNumber;

    [SerializeField] private Slider slider_Music;
    [SerializeField] private Slider slider_SFX;
    [SerializeField] private Slider slider_VideoSFX;
    AudioListener audioListener;
    private int music_Volumn;
    private int sfx_Volumn;
    private int video_SFXVolumn;
    private void Start()
    {
        if (slider_Music != null || slider_SFX != null || slider_VideoSFX != null)
        {


            slider_Music.value = TPRLSoundManager.musicVolume;
            slider_SFX.value = TPRLSoundManager.soundFXVolume;
            slider_VideoSFX.value = TPRLSoundManager.videoVolume;
        }
    }

    public void SetText(TMP_Text tmp_text, string value)
    {
        if (tmp_text != null)
            tmp_text.text = value;
    }
    public void DefaultSettingSound()
    {
        if (slider_Music != null)
            slider_Music.value = 1;

        if (slider_SFX != null)
            slider_SFX.value = 1;

        if (slider_VideoSFX != null)
            slider_VideoSFX.value = 1;

        ChangeMusicVolumnBySlider();
        ChangeSFXVolumnBySlider();
        ChangeVideoSFXVolumnBySlider();
    }
    public void ChangeMusicVolumnBySlider()
    {
        if (slider_Music == null) return;
        decimal deMusic_Volumn = Math.Round((decimal)slider_Music.value,2);
        music_Volumn = (int)(deMusic_Volumn * 100);
        SetText(tx_MusicVolumnNumber, music_Volumn.ToString());
        TPRLSoundManager.Instance.SetMusicVolme((float) deMusic_Volumn);
    }

    public void ChangeSFXVolumnBySlider()
    {
        if (slider_SFX == null) return;
        decimal deSfx_Volumn = Math.Round((decimal)slider_SFX.value, 2);
        sfx_Volumn = (int)(deSfx_Volumn * 100);
        SetText(tx_SFXVolumnNumber, sfx_Volumn.ToString());
        TPRLSoundManager.Instance.SetVolumeSFX ((float)deSfx_Volumn);
    }


    public void ChangeVideoSFXVolumnBySlider()
    {
        if (slider_VideoSFX == null) return;
        decimal deVideo_SfxVolumn = Math.Round((decimal)slider_VideoSFX.value, 2);
        video_SFXVolumn = (int)(deVideo_SfxVolumn * 100);
        TPRLSoundManager.Instance.SetVolumeVideo((float)deVideo_SfxVolumn);
        SetText(tx_VideoSFXVolumnNumber, video_SFXVolumn.ToString());
    }
}