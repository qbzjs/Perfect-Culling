using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponceATMInteract : ResponseInteraction
{
    public override string InteractionTypeEffect => "InteractATM";
    public override string InteractionType => "Go To " + name;
}
