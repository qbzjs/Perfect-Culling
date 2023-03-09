using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;

public class TextRunALine : MonoBehaviour
{
    [SerializeField] ObscuredString urlImage;
    [SerializeField] private Image textRun;
    [SerializeField] float timeRunText = 10f;
    [SerializeField] private Canvas canvasParent;
    private float positionXBegin;
    private float positionXEnd;
    private float width;
    private float widthParent;
    private float heightParent;
    private float posXTrigger = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (urlImage != null)
        {
            widthParent = canvasParent.GetComponent<RectTransform>().rect.width;
            heightParent = canvasParent.GetComponent<RectTransform>().rect.height;
            if (urlImage != null &&!string.IsNullOrEmpty(urlImage))
            {
                RegisterTexture(urlImage, textRun);

            }
            textRun.SetNativeSize();
            width = (float)textRun.sprite.rect.width * heightParent / textRun.sprite.rect.height;
            positionXBegin = width / 2 + widthParent / 2;
            positionXEnd = -widthParent / 2 - width / 2;
            textRun.rectTransform.sizeDelta = new Vector2(width, heightParent);
            textRun.rectTransform.anchoredPosition = new Vector2(positionXBegin, 0);
        }
        RunText();
        
    }


    private void RunText()
    {
        textRun.transform.LeanMoveLocalX(positionXEnd, timeRunText).setOnComplete(() =>
        {
            textRun.rectTransform.anchoredPosition = new Vector2(positionXBegin, 0);
            RunText();
        });
    }

    private void RegisterTexture(string url, Image image)
    {
        ImageManager.instance.RegisterImage(url, (url, texture) =>
        {
            image.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        });
    }

}
