using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSitEffect : MonoBehaviour, IInteractionEffect
{
    private void Awake()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Escape, "StandUp", StandUp, ActionKeyType.Up);
    }
    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Escape, "StandUp", StandUp, ActionKeyType.Up);
    }

    private void Start()
    {
        
    }

    public void Init(GameObject ob1, ResponseInteraction ob2)
    {
        ResponseSeatInteractionComponent seat_info = (ResponseSeatInteractionComponent)ob2;
        int[] seat_positions = seat_info.recordSeatInteractionInfo.seat_positions;
        Debug.LogError("seat_positions : " + (seat_positions != null) + " - " + seat_positions[0]);
        //do seat
    }

    private void StandUp()
    {
        Close();
    }

    private void Close()
    {
        Destroy(gameObject);
    }

    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        
    }

    public void OnDone()
    {
        
    }
}
