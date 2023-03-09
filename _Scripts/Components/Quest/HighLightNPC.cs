using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightNPC : MonoBehaviour
{
    [SerializeField] private ParticleSystem highlightEffect;
    void Start()
    {
        highlightEffect.Play();
    }
}
