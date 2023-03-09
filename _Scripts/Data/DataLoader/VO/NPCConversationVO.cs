using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConversationVO : BaseMutilVO
{
    public NPCConversationVO()
    {
        LoadDataByDirectories<BaseVO>("NPC");
    }
}
