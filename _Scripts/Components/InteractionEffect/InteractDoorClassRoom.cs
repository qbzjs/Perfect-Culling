using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDoorClassRoom : MonoBehaviour, IInteractionEffect
{
    private Action onDone;
    private CharacterController characterController;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        if (popUpNotice != null)
        {
            popUpNotice.OnSetTextTwoButtonCustom(Constant.NOTICE, "Would you wish to exit class ?", ExitClass, OnDone, "Exit", "Stay");
        }
        else
            ExitClass();

        characterController = ob1.GetComponent<CharacterController>();
        if (characterController != null)
            characterController.enabled = false;
        Ultis.SetActiveCursor(true);
    }

    private void ExitClass()
    {
        PopupChangeScene popupChangeScene = PanelManager.Show<PopupChangeScene>();
        popupChangeScene.Init(0, 2);
        NetworkingManager.Instance.classRoom.Leave();
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayMain, false, delegate
        {
            popupChangeScene.Init(1, 2);
            Ultis.SetActiveCursor(false);
            ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.GamePlayCanvas, true, delegate
            {
                NetworkingManager.OnMessage<RecordChatHistory>(EventName.GET_CHAT_HISTORY, (msg) =>
                {
                    UserDatas.record_chat_history = msg.chat_histories.ToArray();
                });
                popupChangeScene.Init(2, 2);
            }, true);
        }, true);

        OnDone();
    }


    public void OnDone()
    {
        Ultis.SetActiveCursor(false);
        if (characterController != null)
            characterController.enabled = true;
        onDone?.Invoke();
    }
}
