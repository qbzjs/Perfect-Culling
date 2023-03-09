using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("me"))
        {
            Observer.Instance.Notify(ObserverKey.Teleport);
        }
    }
}
