using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public void Init(GameObject ob1, ResponseInteraction ob2)
    {
        if(ob1 == null || ob2 == null)
        {
            Destroy(gameObject);
            return;
        }
        string component_type = ob2.InteractionTypeEffect;
        if (string.IsNullOrEmpty(component_type))
        {
            Destroy(gameObject);
            return;
        }
        SetlockInput(true);
        Type type = Type.GetType(component_type);
        IInteractionEffect interactionEffect = (IInteractionEffect)gameObject.AddComponent(type);
        interactionEffect.Init(ob1, ob2, delegate { OnDone(); });
    }

    private void OnDone()
    {
        SetlockInput(false);
        Destroy(gameObject);
    }

    private void SetlockInput(bool is_lock)
    {
        GameConfig.gameBlockInput = is_lock;
    }


}
