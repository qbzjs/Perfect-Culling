using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class InteractVideoOnBuilding : MonoBehaviour, IInteractionEffect
{
    Action action;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        bool is_audio_mute = TPRLSoundManager.Instance.GetStatusAudioSource(ob2.name);
        TPRLSoundManager.Instance.MuteSoundVideo(ob2.name, !is_audio_mute);
        action = on_done;
        OnDone();
    }

    public void OnDone()
    {
        action?.Invoke();
    }


}
