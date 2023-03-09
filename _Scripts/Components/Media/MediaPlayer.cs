using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaPlayer : MonoBehaviour
{
    private void Step()
    {
        if (LayerMask.NameToLayer("NPC") == gameObject.layer) return;
        TPRLSoundManager.Instance.PlaySoundFxOnGameObject(gameObject,"run");
    }
    int clapCount = 0;
    private void Clap()
    {
        if (clapCount == 11)
        {
            clapCount = 0;
        }
        clapCount++;
        TPRLSoundManager.Instance.PlaySoundFxOnGameObject(gameObject, "clap" + clapCount);
    }
}
