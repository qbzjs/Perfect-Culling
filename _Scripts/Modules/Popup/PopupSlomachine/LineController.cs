using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LineController : MonoBehaviour
{
    private UILineRenderer _uiLineRenderer;

    private UILineRenderer uILineRenderer
    {
        get
        {
            if (_uiLineRenderer == null) _uiLineRenderer = GetComponent<UILineRenderer>();
            return _uiLineRenderer;
        }
    }

    [SerializeField] private Sprite[] sprites;

    private int animationStep;

    [SerializeField] private float fps = 30f;

    private float fpsCounter;
    private void Update()
    {
        PlayAnimationSprite();
    }
    private void PlayAnimationSprite()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {

            animationStep++;
            if (animationStep == sprites.Length)
            {
                animationStep = 0;
            }
            uILineRenderer.sprite = sprites[animationStep];
            fpsCounter = 0;
        }
    }
}
