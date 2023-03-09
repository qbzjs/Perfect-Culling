using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardClassQuestion : MonoBehaviour
{
    [SerializeField] private List<TopClassQuestion> topClassQuestions;

    public Dictionary<string, ClassQuestionRecord> listClassQuestionResult = new Dictionary<string, ClassQuestionRecord>();


    public void ShowLeaderBoard()
    {
        List<ClassQuestionRecord> lst = new List<ClassQuestionRecord>();
        foreach(var result in listClassQuestionResult)
        {
            lst.Add(result.Value);
        }
        lst.Sort((x, y) => x.point.CompareTo(y.point));
        for (int i = 0; i < lst.Count && i<5; i++)
        {
            topClassQuestions[i].SetInfo(lst[i]);
        }
    }
}
