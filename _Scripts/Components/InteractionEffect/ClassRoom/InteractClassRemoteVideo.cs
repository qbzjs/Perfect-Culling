using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractClassRemoteVideo : MonoBehaviour, IInteractionEffect
{
    Action action;
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        action = on_done;
        EntityManager entityManager = ob1.GetComponent<EntityManager>();
        if (entityManager == null || entityManager.classRole != ClassRoomRole.teacher)
        {
            OnDone();
            return;
        }
        NetworkingManager.NetSend(EventName.SWITCH_PROJECTOR_MODE, ClassRoomMediaType.video.ToString());
      
        OnDone();
    }

    public void OnDone()
    {
        action?.Invoke();
    }
}
