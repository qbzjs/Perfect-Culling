using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractClassRoom : MonoBehaviour, IInteractionEffect
{
    private Action onDone;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
        CharacterController characterController = ob1.GetComponent<CharacterController>();

        PopupJoinClassRoom popupJoinClassRoom = PanelManager.Show<PopupJoinClassRoom>();
        popupJoinClassRoom.onDone = delegate {
            characterController.enabled = true;
            OnDone();
        };
        characterController.enabled = false;
        Ultis.SetActiveCursor(true);
    }

    public void OnDone()
    {
        onDone?.Invoke();
    }
}
