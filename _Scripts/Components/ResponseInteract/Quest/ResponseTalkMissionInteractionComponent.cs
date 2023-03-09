using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseTalkMissionInteractionComponent : ResponseMissionInteractionComponent
{
    public override string InteractionTypeEffect => "InteractTalkMissionEffect";
    public override string InteractionType => "Talk";
}
