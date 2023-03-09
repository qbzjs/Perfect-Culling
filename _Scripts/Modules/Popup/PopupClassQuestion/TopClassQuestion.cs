using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopClassQuestion : MonoBehaviour
{
    [SerializeField] private Image topImage;
    [SerializeField] private TMP_Text topName;
    [SerializeField] private TMP_Text topPoint;

    public void SetInfo(ClassQuestionRecord record)
    {
        GetTexture(record.urlAvatarImage);
        topName.text = record.name;
        topPoint.text = $"{record.point}";
    }
    private void GetTexture(string url)
    {
        ImageManager.instance.RegisterImage(url, (url, texture) =>
        {
            if (texture != null)
                topImage.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        });
        ImageManager.instance.StartDownloadImage();
    }
}
