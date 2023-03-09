using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScholarNPCController : GeneralNPCController
{
    private NPCAnimationEvent _NPCAnimationEvent;
    private NPCAnimationEvent nPCAnimationEvent
    {
        get
        {
            if (_NPCAnimationEvent == null)
            {
                _NPCAnimationEvent = GetComponentInChildren<NPCAnimationEvent>();
            }
            return _NPCAnimationEvent;
        }
    }
    protected override void Start()
    {
        base.Start();
        if (npc_Agent != null)
            npc_Agent.speed = 2*move_Speed;
    }
    protected override void MoveTotarget()
    {
        if (npc_Agent != null)
        {
            if (Vector3.Distance(npc_Agent.transform.position, position) <= 10)
            {
                position = GetRandomWayPoint();
            }
            else
            {
                npc_Agent.SetDestination(position);
                SetAnim("isMoving", true);
                SetAnim("isSprint", true);
            }
            RandomFallFlat();
        }
    }
    private void RandomFallFlat()
    {
        int rd = Random.Range(1, 100);
        if(rd == 1)
        {
            npc_Agent.isStopped = true;
            SetAnim("isFallAhead", true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("LayerChat"))
        {
            string conversationId = "conversation_1";
            NPCConversation conversation = DataController.Instance.NPC_Conversation_VO.GetDataByName<NPCConversation>("NPCConversation", conversationId);
            CancelInvoke();
            nPCAnimationEvent.isCollideWithPlayer = true;
            nPCAnimationEvent.targetTransform = other.transform;
            npc_Agent.isStopped = true;
            SetAnim("isFallAhead", true);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            //TODO: Talk to other NPC
        }
    }
}
