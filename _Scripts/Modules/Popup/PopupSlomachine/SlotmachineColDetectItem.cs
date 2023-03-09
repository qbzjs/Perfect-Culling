using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlotmachineColDetectItem : MonoBehaviour
{
    [HideInInspector]
    public bool isStartDetect = false;
    public UnityAction<string> actionDetect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStartDetect == false) return;
        actionDetect?.Invoke(collision.name);
    }
}
