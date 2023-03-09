using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassQuestionAnswerLocation : MonoBehaviour
{
    [SerializeField] private int number;
    public bool isCorrectAnswer;
    public bool isTeacher;
    private List<EntityManager> listEntityCollide = new List<EntityManager>();
    [SerializeField] private ParticleSystem correctEffect;
    private void OnTriggerEnter(Collider other)
    {
        EntityManager entityManager = other.GetComponent<EntityManager>();
        if(!listEntityCollide.Contains(entityManager))
        {
            listEntityCollide.Add(entityManager);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        EntityManager entityManager = other.GetComponent<EntityManager>();
        if (listEntityCollide.Contains(entityManager))
        {
            listEntityCollide.Remove(entityManager);
        }
    }
    private void OnDisable()
    {
        
    }
    public void PlayCorrectEffect()
    {
        correctEffect.gameObject.SetActive(true);
        correctEffect.Play();
    }
    public void StopCorrectEffect()
    {
        correctEffect.Stop();
        correctEffect.gameObject.SetActive(false);
    }
    public void CheckResult()
    {
        string emotion_name = isCorrectAnswer? "Clap 2": "Dizzy Idle";
        if (isCorrectAnswer) PlayCorrectEffect();
        foreach (EntityManager entityManager in listEntityCollide)
        {
            Animator animator = entityManager.transform.GetChild(0).GetComponent<Animator>();
            if (isCorrectAnswer)
            {
                entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION3;
            }
            else
            {
                entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION2;
            }
            animator.Play(emotion_name);
        }
        if (isTeacher)
        {
            PopupClassQuestion popupClassQuestion = PanelManager.Show<PopupClassQuestion>();
            popupClassQuestion.QuestionBox.SetActive(false);
            if (isCorrectAnswer)
            {
                foreach (EntityManager entityManager in listEntityCollide)
                {
                    if (!popupClassQuestion.leaderBoard.listClassQuestionResult.ContainsKey(entityManager.ID))
                    {
                        ClassQuestionRecord record = new ClassQuestionRecord();
                        record.urlAvatarImage = "";
                        record.name = entityManager.Name;
                        record.point = 10;
                        popupClassQuestion.leaderBoard.listClassQuestionResult.Add(entityManager.ID, record);
                    }
                    else
                    {
                        int point = popupClassQuestion.leaderBoard.listClassQuestionResult[entityManager.ID].point;
                        ClassQuestionRecord record = new ClassQuestionRecord();
                        record.urlAvatarImage = "";
                        record.name = entityManager.Name;
                        record.point = point + 10;
                        popupClassQuestion.leaderBoard.listClassQuestionResult[entityManager.ID] = record;
                    }
                }
            }
            popupClassQuestion.leaderBoard.ShowLeaderBoard();
        }
    }
}
