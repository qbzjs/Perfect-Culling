using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemBillboard : MonoBehaviour
{
    private Image _image;
    private Image image
    {
        get
        {
            if (_image == null) _image = GetComponent<Image>();
            return _image;
        }
    }

    private UnityAction<string> onClick;

    private void Start()
    {
        if (image != null)
            image.GetComponent<Button>().onClick.AddListener(Click);
    }

    private RecordLinkBillBoard recordLinkBillBoard;

    public void SetInfo(RecordLinkBillBoard recordLinkBillBoard, UnityAction<string> on_click)
    {
        onClick = on_click;
        this.recordLinkBillBoard = recordLinkBillBoard;
        GetTexture(this.recordLinkBillBoard.urlImage);
    }

    private void GetTexture(string url)
    {
        ImageManager.instance.RegisterImage(url, (url, texture) =>
        {
            if (texture != null)
                image.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
        });
        ImageManager.instance.StartDownloadImage();
    }

    private void Click()
    {
        onClick?.Invoke(recordLinkBillBoard.urlLink);
    }

}
