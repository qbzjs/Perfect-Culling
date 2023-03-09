using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlayerController : MonoBehaviour
{
    [SerializeField] private Button bt_Randomize;
    [SerializeField] private Button[] bt_ChangeColor;
    [SerializeField] private Button bt_Confirm;

    void Start()
    {
        if (bt_Confirm != null)
        {
            bt_Confirm.onClick.AddListener(ClickConfirm);
        }
    }
    private void ClickConfirm()
    {

            PopupLoadingLogin popupLoadingLogin = PanelManager.Show<PopupLoadingLogin>();
            PanelManager.Hide<PopupLoading>();
            ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayMain, false, delegate
            {
                popupLoadingLogin.EventSlider90Percent();
                ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayCanvas, true, delegate
                {
                    popupLoadingLogin.EventSlider100percent();
                    popupLoadingLogin.PlayVideo();
                    var builder = new UriBuilder(EnvironmentConfig.linkServerVoice);
                    var uri = builder.Uri;
                    Mirror.NetworkManager.singleton.StartClient(uri);
                }, true);
            }, true);
            bt_Confirm.interactable =false;
        
       
    }

    private void SetActionToButton(Button bt, RecordItemEquip itemEquip)
    {
        if (bt != null)
            bt.onClick.AddListener(() => { Observer.Instance.Notify(ObserverKey.EquidPart, itemEquip); });
    }

    private void ChangeColor(int id)
    {
        ColorSkin[] skin = DataController.Instance.ColorVO.GetDatasByName<ColorSkin>("CreateCharactersSkin");
        for (int i = 0; i < skin.Length; i++)
        {
            if (id == i)
            {
                Color color;
                ColorUtility.TryParseHtmlString(skin[i].color_code, out color);
            }
        }
    }
    private void Randomize()
    {
        if (bt_ChangeColor != null && bt_ChangeColor.Length > 0)
        {
            //int index_color_random = Random.Range(0, bt_ChangeColor.Length);
            //ChangeColor(index_color_random);
        }

    }
}
