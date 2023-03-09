using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseQuizInteractionComponent : ResponseInteraction
{
    private RecordQuizInteractionInfo _recordQuizInteractionInfo;
    public RecordQuizInteractionInfo recordQuizInteractionInfo (string quiz_id)
    {
        
            _recordQuizInteractionInfo = DataController.Instance.QuizVO.GetDataByName<RecordQuizInteractionInfo>("QuizInfo", quiz_id);
        return _recordQuizInteractionInfo;
    }
    private RecordQuizConversationInfo _recordQuizConversationInfo;
    public RecordQuizConversationInfo recordQuizConversationInfo
    {
        get
        {
            if (_recordQuizConversationInfo.Equals((RecordQuizConversationInfo)default))
                _recordQuizConversationInfo = DataController.Instance.QuizVO.GetDataByName<RecordQuizConversationInfo>("QuizConversationInfo", "quiz_conversation");
            return _recordQuizConversationInfo;
        }
    }

    public override string InteractionTypeEffect => "InteractQuizEffect";
    public override string InteractionType => "Talk";
}
