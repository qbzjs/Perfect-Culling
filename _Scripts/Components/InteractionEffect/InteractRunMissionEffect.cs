using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRunMissionEffect : InteractMissionEffect
{
    public override void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;
    }
}
