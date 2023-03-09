using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractQuizEffect : MonoBehaviour, IInteractionEffect
{
    private Action onDone;
    private int countAnswer = 0;
    public RecordMissionDailyInfo record_missionDaily;
    private void SetRecordQuiz(object data)
    {
        record_missionDaily = (RecordMissionDailyInfo)data;
    }
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.PushRecordMissionQuizDaily, SetRecordQuiz);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PushRecordMissionQuizDaily, SetRecordQuiz);
    }

    public void Init(GameObject ob1, ResponseInteraction ob2, Action on_done)
    {
        onDone = on_done;

        ResponseQuizInteractionComponent responseQuizInteractionComponent = (ResponseQuizInteractionComponent)ob2;

        RecordQuizConversationInfo recordQuizConversationInfo = responseQuizInteractionComponent.recordQuizConversationInfo;

        int randomQuizIndex = UnityEngine.Random.Range(1, 12);

        countAnswer = 0;

        RecordQuizInteractionInfo recordQuizInteractionInfo = responseQuizInteractionComponent.recordQuizInteractionInfo("quiz_" + randomQuizIndex);

        OpenPopupConversation(ob1, recordQuizConversationInfo, recordQuizInteractionInfo,false, bool3.False); 

    }

    public void OnDone()
    {
        Ultis.SetActiveCursor(false);
        Destroy(GetComponent<ResponseQuizInteractionComponent>());
        onDone?.Invoke();
    }
    private void OpenPopupConversation(GameObject ob1, RecordQuizConversationInfo record_quiz_conversation,RecordQuizInteractionInfo record_quiz_interaction_info,bool isAnswered,bool3 isCorrect)
    {
        Ultis.SetActiveCursor(true);

        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        listAvailableKey.Add(KeyCode.Space, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);

        CharacterController characterController = ob1.GetComponent<CharacterController>();
        characterController.enabled = false;
        PopupConversation popupConversation = PanelManager.Show<PopupConversation>();
        GameConfig.gameBlockInput = true;
        if (popupConversation != null)
        {
            if (!isAnswered)
            {
                popupConversation.OnCloseCallback = delegate {
                    PanelManager.Hide<PopupConversation>();
                    OpenPopupQuiz(ob1, record_quiz_conversation, record_quiz_interaction_info);
                };
                string[] conversation = new string[record_quiz_conversation.quiz_position-1];
                for(int i = 0; i < record_quiz_conversation.quiz_position - 1; i++)
                {
                    conversation[i] = record_quiz_conversation.conversation[i];
                }
                popupConversation.InitPopUpConversation(record_quiz_conversation.target_object_name, conversation);
            }
            else
            {
                if (isCorrect == bool3.False)
                {
                    countAnswer++;
                    if(countAnswer == 1)
                    {
                        popupConversation.OnCloseCallback = delegate {
                            PanelManager.Hide<PopupConversation>();
                            OpenPopupQuiz(ob1, record_quiz_conversation, record_quiz_interaction_info);
                        };
                        string[] conversation = new string[1];
                        conversation[0] = "Wrong Answer! You have one more chance to answer this question !";
                        popupConversation.InitPopUpConversation(record_quiz_conversation.target_object_name, conversation);
                    }
                    else if(countAnswer == 2)
                    {
                        popupConversation.OnCloseCallback = delegate {
                            PanelManager.Hide<PopupConversation>();
                            characterController.enabled = true;
                            GameConfig.gameBlockInput = false;
                            OnDone();
                        };
                        string[] conversation = new string[1];
                        conversation[0] = "Have luck next time!";
                        popupConversation.InitPopUpConversation(record_quiz_conversation.target_object_name, conversation);
                    }
                }
                else
                {
                    popupConversation.OnCloseCallback = delegate {
                        PanelManager.Hide<PopupConversation>();
                        characterController.enabled = true;
                        GameConfig.gameBlockInput = false;
                        OnDone();
                        RecordInteractionTypeObserver interaction_type = new RecordInteractionTypeObserver();
                        interaction_type.interaction_type = ResponseInteractionType.Talk;
                        interaction_type.object_name = "Mia Agent";
                        Observer.Instance.Notify(ObserverKey.UpdateResponseInteractionInfo, interaction_type);
                    };
                    int stringRemainingCount = record_quiz_conversation.conversation.Length - record_quiz_conversation.quiz_position + 1;
                    string[] conversation = new string[stringRemainingCount + 1];

                    int firstStringRemainingIndex = record_quiz_conversation.quiz_position - 1;

                    if (isCorrect == bool3.True)
                    {
                        conversation[0] = "Congratulation!";
                        for (int i = firstStringRemainingIndex; i < record_quiz_conversation.conversation.Length; i++)
                        {
                            conversation[i - firstStringRemainingIndex + 1] = record_quiz_conversation.conversation[i];

                        }
                        CompleteQuestQuiz();
                    }
                    else if (isCorrect == bool3.Neutral)
                    {
                        conversation[0] = "Thank for your answer !";
                        //TODO : Collect Data :vvvv
                        for (int i = firstStringRemainingIndex; i < record_quiz_conversation.conversation.Length; i++)
                        {
                            conversation[i - firstStringRemainingIndex + 1] = record_quiz_conversation.conversation[i];
                        }
                        CompleteQuestQuiz();
                    }
                    popupConversation.InitPopUpConversation(record_quiz_conversation.target_object_name, conversation);
                }
            }
        }
    }
    private void CompleteQuestQuiz()
    {
        QuestManager.CompleteQuestDaily(record_missionDaily,true);
        Destroy(GetComponent<ResponseQuizInteractionComponent>());
    }
    private void OpenPopupQuiz(GameObject ob1,RecordQuizConversationInfo record_quiz_conversation, RecordQuizInteractionInfo record_quiz_interaction_info)
    {
        Ultis.SetActiveCursor(true);
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        PopupQuiz popupQuiz = PanelManager.Show<PopupQuiz>();
        if (popupQuiz != null)
        {
            popupQuiz.OnCompleteQuizAction = (isCorrect) => {
                PanelManager.Hide<PopupQuiz>();
                OpenPopupConversation(ob1, record_quiz_conversation, record_quiz_interaction_info,true, isCorrect);
            };
            popupQuiz.InitPopupQuizProperties(record_quiz_conversation.target_object_name, record_quiz_interaction_info);
        }
    }
}
