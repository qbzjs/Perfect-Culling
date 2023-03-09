using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractParagonShelf : MonoBehaviour, IInteractionEffect
{
    Action action;
    ParagonInfoShowUp info;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        info = ob2.GetComponent<ParagonInfoShowUp>();
        Application.OpenURL("https://theparallel.io/market/shop/paragon");
        action = on_done;
    }

    private void OnInteract(ParagonInfoShowUp info)
    {
        info.DestroyCurrentParagon();
        info.CreateNewParagon();
    }
    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            OnInteract(info);
            OnDone();
        }
    }
    public void OnDone()
    {
        action?.Invoke();
    }


}
