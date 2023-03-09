using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseTalkInteractionComponent : ResponseInteraction
{

    public override string InteractionTypeEffect => "InteractTalkEffect";
    public override string InteractionType => "Talk";
}
