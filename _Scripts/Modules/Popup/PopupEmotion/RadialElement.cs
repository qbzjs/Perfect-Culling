using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialElement : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private void OnEnable()
    {
        SetDefaultColor();
    }
    public Image EmotionBGImage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlightColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultColor();
    }
    private void SetDefaultColor()
    {
        EmotionBGImage.color = new Color32(61, 61, 61, 90);
    }
    private void SetHighlightColor() {
        EmotionBGImage.color = new Color32(0, 140, 255, 90);
    }
}
