using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractToAscention : MonoBehaviour, IInteractionEffect
{
    Action action;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        Application.OpenURL(EnvironmentConfig.linkGameAscention);
        action = on_done;
        OnDone();
    }

    public void OnDone()
    {
        action?.Invoke();
    }


}
