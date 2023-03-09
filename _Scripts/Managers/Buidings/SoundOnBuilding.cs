using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SoundOnBuilding : MonoBehaviour
{
    [SerializeField] private AudioSource audio_Source;
    [SerializeField] private VideoPlayer video_Player;
    [SerializeField] private float radius = 0;
    private void Start()
    {
        if (audio_Source != null)
        {
            TPRLSoundManager.Instance.RegisterVideoSound(gameObject.name, audio_Source);
            TPRLSoundManager.Instance.MuteSoundVideo(gameObject.name, true);
            audio_Source.spatialBlend = 1;
            audio_Source.maxDistance = radius;
            if (video_Player != null)
            {
                video_Player.SetTargetAudioSource(0, audio_Source);
            }
        }
    }
}
