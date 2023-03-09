using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassQuestionObject : MonoBehaviour
{
    public List<ClassQuestionAnswerLocation> listLocations;
    [SerializeField] List<ParticleSystem> quizLocationsPS;
    public bool isNewQuestion =true;
    private void Start()
    {
        NetworkingManager.OnMessage<object>(EventName.END_QUIZ, (msg) =>
        {
            if (!isNewQuestion) return;
            PanelManager.Hide<PopupClassQuestion>();
            foreach (ClassQuestionAnswerLocation location in listLocations)
                location.CheckResult();

            isNewQuestion = false;
        });
    }
    private void OnEnable()
    {
        foreach(ParticleSystem ps in quizLocationsPS)
        {
            ps.Play();
        }
    }
}
