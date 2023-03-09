using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionEffect
{
    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done);
    public void OnDone();
}
