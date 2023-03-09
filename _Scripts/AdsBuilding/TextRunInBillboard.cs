using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;

public class TextRunInBillboard : MonoBehaviour
{
    [SerializeField] ObscuredString[] urlImage;
    [SerializeField] private Image[] textRun;
    [SerializeField] float timeRunText = 10f;
    [SerializeField] private Canvas canvasParent;
    private float positionXBegin;
    private float positionXEnd;
    private float width;
    private float widthParent;
    private float heightParent;
    private int indexImage = 0;
    private float posXTrigger = 0;
    private float time1 = 0;
    private float time2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        widthParent = canvasParent.GetComponent<RectTransform>().rect.width;
        heightParent = canvasParent.GetComponent<RectTransform>().rect.height;
        if (urlImage != null && urlImage.Length > 0)
        {
            int lenghtUrl = urlImage.Length;
            for (int i = 0; i < lenghtUrl; i++)
            {
                if (urlImage[i] != null && !string.IsNullOrEmpty(urlImage[i]))
                {
                    RegisterTexture(urlImage[i], textRun[i]);
                }
            }
        }
        if (textRun != null && textRun.Length > 0)
        {
            int lenght = textRun.Length;

            for (int i = 0; i < lenght; i++)
            {
                textRun[i].SetNativeSize();
            }

            width = (float)textRun[0].sprite.rect.width * heightParent / textRun[0].sprite.rect.height;
            positionXBegin = width / 2 + widthParent / 2;
            positionXEnd = -widthParent / 2 - width / 2;
            posXTrigger = -widthParent / 2 + width / 2;
            time1 = (float)widthParent / (width + widthParent) * timeRunText;
            time2 = timeRunText - time1;

            for (int i = 0; i < 4; i++)
            {
                textRun[i].rectTransform.sizeDelta = new Vector2(width, heightParent);
                textRun[i].rectTransform.anchoredPosition = new Vector2(positionXBegin, 0);
            }
            Run(indexImage);
        }
    }

    private void IncreaseIndexImage()
    {
        indexImage++;
        if (indexImage >= textRun.Length)
            indexImage = 0;
    }

    private void Run(int index)
    {
        Image image = textRun[index];
        IncreaseIndexImage();
        image.rectTransform.LeanMoveLocal(new Vector3(posXTrigger, 0, 0), time1).setEaseLinear().setOnComplete(() =>
        {
            Run(indexImage);
            image.rectTransform.LeanMoveLocal(new Vector3(positionXEnd, 0, 0), time2).setEaseLinear().setOnComplete(() =>
            {
                image.rectTransform.anchoredPosition = new Vector2(positionXBegin, 0);
            });
        });

    }
    private void RegisterTexture(string url, Image image)
    {
        ImageManager.instance.RegisterImage(url, (url, texture) =>
        {
            if (texture != null)
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        });
    }

}
