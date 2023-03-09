using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneralNPCController : MonoBehaviour
{
    [SerializeField] private WaypointGroup way_Point;
    protected NavMeshAgent npc_Agent;
    public float move_Speed;
    [SerializeField] private Animator animator;
    [SerializeField] private string name_of_postion_target;
    protected Vector3 position;

    protected virtual void Start()
    {
        npc_Agent = GetComponent<NavMeshAgent>();
        if (npc_Agent != null)
            npc_Agent.speed = move_Speed;
        position = GetRandomWayPoint();
    }

    public void Init(WaypointGroup way_point)
    {
        way_Point = way_point;
        InvokeRepeating("MoveTotarget", 1, 1); ;
    }

    protected Vector3 GetRandomWayPoint()
    {
        int random_number = Random.Range(0, way_Point.lst_WayPoint.Count);
        do
        {
            if (random_number == previous_Target)
            {
                random_number = Random.Range(0, way_Point.lst_WayPoint.Count);
            }
            else
            {
                break;
            }
        } while (true);

        name_of_postion_target = way_Point.lst_WayPoint[random_number] + "";
        previous_Target = random_number;
        return way_Point.lst_WayPoint[random_number].position;
    }
    int previous_Target = 0;
    protected virtual void MoveTotarget()
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
            }
        }
    }
    protected void SetAnim(string name_of_anim_clip, bool active)
    {
        animator.SetBool(name_of_anim_clip, active);
    }
}
