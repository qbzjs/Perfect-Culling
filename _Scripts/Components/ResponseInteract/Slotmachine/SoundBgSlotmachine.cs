using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBgSlotmachine : MonoBehaviour
{
    [SerializeField] private AudioSource audio_Source;
    [SerializeField] private float radius = 0;
    private void Start()
    {
        if (audio_Source != null)
        {
            TPRLSoundManager.Instance.RegisterSFX("SoundSlotMachineBG", audio_Source);
            TPRLSoundManager.Instance.PlaySFX("SoundSlotMachineBG", "SlotBackgroundMusic");
            audio_Source.spatialBlend = 1;
            audio_Source.maxDistance = radius;
        }
    }
}