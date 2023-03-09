using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageBillBoard : MonoBehaviour
{
    RecordLinkBillBoard [] recordLinkBillBoard= new RecordLinkBillBoard[11];
    [SerializeField] private ItemBillboard[] itemBillboards;

    void Start()
    {

        SetFake();
        if (itemBillboards != null)
        {
            int length = itemBillboards.Length;
            for (int i = 0; i < length; i++)
            {
                ItemBillboard item = itemBillboards[i];
                item.SetInfo(recordLinkBillBoard[i], ClickItem);
            }
        }
    }

    private void ClickItem(string url)
    {
        Application.OpenURL(url);
    }

    void SetFake()
    {
        recordLinkBillBoard[0].urlImage = "https://cafefcdn.com/thumb_w/650/pr/2021/photo2021-11-1817-36-04-16372325109501637846214-0-0-778-1246-crop-1637232676751-63772857402763.jpg";
        recordLinkBillBoard[0].urlLink = "https://theparallel.io/";
        recordLinkBillBoard[1].urlImage = "https://tienthuattoan.com/wp-content/uploads/2022/01/Copyright-%C2%A9-tienthuattoan.com_-1.png";
        recordLinkBillBoard[1].urlLink = "https://theparallel.io/market/shop/paragon";
        recordLinkBillBoard[2].urlImage = "https://binancegate.com/wp-content/uploads/2021/11/The-Parallel-thumb.jpg";
        recordLinkBillBoard[2].urlLink = "https://theparallel.io/games";
        recordLinkBillBoard[3].urlImage = "https://cafefcdn.com/thumb_w/650/pr/2021/photo2021-11-1817-36-04-16372325109501637846214-0-0-778-1246-crop-1637232676751-63772857402763.jpg";
        recordLinkBillBoard[3].urlLink = "https://theparallel.io/";
        recordLinkBillBoard[4].urlImage = "https://tienthuattoan.com/wp-content/uploads/2022/01/Copyright-%C2%A9-tienthuattoan.com_-1.png";
        recordLinkBillBoard[4].urlLink = "https://theparallel.io/market/shop/paragon";
        recordLinkBillBoard[5].urlImage = "https://binancegate.com/wp-content/uploads/2021/11/The-Parallel-thumb.jpg";
        recordLinkBillBoard[5].urlLink = "https://theparallel.io/games";
        recordLinkBillBoard[6].urlImage = "https://cafefcdn.com/thumb_w/650/pr/2021/photo2021-11-1817-36-04-16372325109501637846214-0-0-778-1246-crop-1637232676751-63772857402763.jpg";
        recordLinkBillBoard[6].urlLink = "https://theparallel.io/";
        recordLinkBillBoard[7].urlImage = "https://tienthuattoan.com/wp-content/uploads/2022/01/Copyright-%C2%A9-tienthuattoan.com_-1.png";
        recordLinkBillBoard[7].urlLink = "https://theparallel.io/market/shop/paragon";
        recordLinkBillBoard[8].urlImage = "https://binancegate.com/wp-content/uploads/2021/11/The-Parallel-thumb.jpg";
        recordLinkBillBoard[8].urlLink = "https://theparallel.io/games";
        recordLinkBillBoard[9].urlImage = "https://cafefcdn.com/thumb_w/650/pr/2021/photo2021-11-1817-36-04-16372325109501637846214-0-0-778-1246-crop-1637232676751-63772857402763.jpg";
        recordLinkBillBoard[9].urlLink = "https://theparallel.io/";
        recordLinkBillBoard[10].urlImage = "https://tienthuattoan.com/wp-content/uploads/2022/01/Copyright-%C2%A9-tienthuattoan.com_-1.png";
        recordLinkBillBoard[10].urlLink = "https://theparallel.io/market/shop/paragon";

    }


    
}
