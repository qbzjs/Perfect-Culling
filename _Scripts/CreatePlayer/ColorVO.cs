using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorVO : BaseMutilVO
{
    public ColorVO()
    {
        LoadDataByDirectories<BaseVO>("Characters");
    }
}