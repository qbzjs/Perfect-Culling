using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("PopupClassQuestion", "PopupCanvas")]
public class PopupClassQuestion : BasePanel
{
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private List<TMP_Text> answerText;
    [SerializeField] private TMP_Text timeRemainingText;
    [SerializeField] private List<Image> correctAnswerImages;
    public GameObject QuestionBox;
    public LeaderBoardClassQuestion leaderBoard;

    private int correctAnswer = 0;
    public void Init(RecordQuizInteractionInfo quiz,bool isTeacher)
    {
        QuestionBox.SetActive(true);
        if (!isTeacher) leaderBoard.gameObject.SetActive(true);
        correctAnswer = quiz.correct_answer;
        SetQuestionText(quiz.question);
        SetAnswerText(quiz.answer);
    }

    private readonly string headerQuestionText = "Di chuyển vào ô chứa đáp án đúng\n";
    private void SetQuestionText(string question)
    {
        questionText.text = $"{headerQuestionText}{question}";
    }
    private void SetAnswerText(string[] answers)
    {
        int i=0;
        for (; i < answers.Length; i++)
        {
            answerText[i].text = answers[i];
            answerText[i].transform.parent.gameObject.SetActive(true);
        }
        for (; i < answerText.Count; i++)
        {
            answerText[i].transform.parent.gameObject.SetActive(false);
        }
    }
    public void ShowResult()
    {
        StartCoroutine(IEShowResult());
    }
    private IEnumerator IEShowResult()
    {
        correctAnswerImages[correctAnswer-1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        ResetAnswer();
        HidePanel();
    }
    private void ResetAnswer()
    {
        foreach(Image image in correctAnswerImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}
