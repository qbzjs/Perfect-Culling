using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseSeatInteractionComponent : ResponseInteraction
{
    private RecordSeatInteractionInfo _recordSeatInteractionInfo;
    public RecordSeatInteractionInfo recordSeatInteractionInfo
    {
        get
        {
            if (_recordSeatInteractionInfo.Equals((RecordSeatInteractionInfo)default))
                _recordSeatInteractionInfo = DataController.Instance.Seat_VO.GetDataByName<RecordSeatInteractionInfo>("SeatInfo", "chair");
            return _recordSeatInteractionInfo;
        }
    }

    public override string InteractionTypeEffect => "InteractSitEffect";
    public override string InteractionType => "Sit";
}
