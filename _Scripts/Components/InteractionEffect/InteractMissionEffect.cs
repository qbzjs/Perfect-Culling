using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMissionEffect : MonoBehaviour, IInteractionEffect
{
    protected Action onDone;
    public virtual void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        
    }
    

    private void DestroyInteractObject()
    {
        Destroy(gameObject);
    }

    public void OnDone()
    {
        onDone?.Invoke();
        DestroyInteractObject();
    }
}
