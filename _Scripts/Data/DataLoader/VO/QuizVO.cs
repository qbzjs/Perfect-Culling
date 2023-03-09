using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizVO : BaseMutilVO
{
    public QuizVO()
    {
        LoadDataByDirectories<BaseVO>("Quiz");
    }
}
