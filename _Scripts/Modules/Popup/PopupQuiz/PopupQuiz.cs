using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[UIPanelPrefabAttr("PopupQuiz", "PopupCanvas")]
public class PopupQuiz : BasePanel
{
    [SerializeField] private TMP_Text nPCNameText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private List<Answer> answersList;

    public Action<bool3> OnCompleteQuizAction;

    private int correctAnswer;

    [SerializeField] private List<Sprite> checkboxSprites;

    private void Start()
    {
        InitEvent();
    }
    private void InitEvent()
    {
        answersList[0].checkBox.onClick.AddListener(() => { OnCheckBoxClick(0); });
        answersList[1].checkBox.onClick.AddListener(() => { OnCheckBoxClick(1); });
        answersList[2].checkBox.onClick.AddListener(() => { OnCheckBoxClick(2); });
        answersList[3].checkBox.onClick.AddListener(() => { OnCheckBoxClick(3); });
    }
    private void OnCheckBoxClick(int index)
    {
        StartCoroutine(CheckResult(index));
    }
    private IEnumerator CheckResult(int index)
    {
        answersList[index].checkBox.GetComponent<Image>().sprite = checkboxSprites[1];
        bool3 isCorrect = bool3.False;
        yield return new WaitForSeconds(1);
        if (correctAnswer == 0)
        {
            answersList[index].correctAnswerImage.gameObject.SetActive(true);
            isCorrect = bool3.Neutral;
        }
        else
        {
            if (index == correctAnswer - 1)
            {
                answersList[index].correctAnswerImage.gameObject.SetActive(true);
                isCorrect = bool3.True;
            }
            else
            {
                answersList[index].wrongAnswerImage.gameObject.SetActive(true);
                isCorrect = bool3.False;
            }
        }
        yield return new WaitForSeconds(1);
        answersList[index].checkBox.GetComponent<Image>().sprite = checkboxSprites[0];
        OnCompleteQuizAction?.Invoke(isCorrect);
        PanelManager.Hide<PopupQuiz>();
    }
    public void InitPopupQuizProperties(string npc_name,RecordQuizInteractionInfo record_quiz_interaction_info)
    {
        Reset();
        if (nPCNameText != null)
        {
            nPCNameText.text = npc_name;
            questionText.text = record_quiz_interaction_info.question;
            for(int i = 0; i < record_quiz_interaction_info.answer.Length; i++)
            {
                answersList[i].checkBox.gameObject.SetActive(true);
                answersList[i].answerText.text = record_quiz_interaction_info.answer[i];
            }
            correctAnswer = record_quiz_interaction_info.correct_answer;
        }
    }
    private void Reset()
    {
        foreach (Answer answer in answersList)
        {
            answer.checkBox.gameObject.SetActive(false);
            answer.answerText.text = "";
            answer.correctAnswerImage.gameObject.SetActive(false);
            answer.wrongAnswerImage.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public struct Answer
{
    public Button checkBox;
    public TMP_Text answerText;
    public Image correctAnswerImage;
    public Image wrongAnswerImage;
}
public enum bool3
{
    True,
    False,
    Neutral
}