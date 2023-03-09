using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWheel : MonoBehaviour
{
    [SerializeField] private GameObject lightwheel;
    public void SetStatusLight(bool status)
    {
        lightwheel.SetActive(status);
    }
}
