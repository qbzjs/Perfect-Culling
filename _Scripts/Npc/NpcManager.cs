using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




public class NpcManager : MonoBehaviour
{
    [SerializeField] private List<WaypointGroup> waypoint_Group;
    [SerializeField] private List<GameObject> npc_Model;
    [SerializeField] private float speed_Npc = 1.8f;
    [SerializeField] private int npc_Number;
    [SerializeField] private float spawn_Range;

    private float seekFraction = 0.05f;
    private float seekRange;
    private void Start()
    {
        seekRange = seekFraction * spawn_Range;
        Init();
    }
    private void Init()
    {

        for (int i = 0; i < npc_Number; i++)
        {
            SpawnNpc();

        }

    }

    private void SpawnNpc()
    {
        int npc_model_index = Random.Range(0, npc_Model.Count);
        int waypoint_index = Random.Range(0, waypoint_Group.Count);
        var waypoint_group = waypoint_Group[waypoint_index];
        Vector3 spawn_position = RandomPointOnNavMesh(waypoint_group.lst_WayPoint[0].transform.position, 0);
        if (spawn_position == Vector3.zero) return;
        var npc_object = Instantiate(npc_Model[npc_model_index], spawn_position, Quaternion.identity);
        npc_object.transform.SetParent(transform);
        var npc_controller = npc_object.GetComponent<GeneralNPCController>();
        npc_controller.Init(waypoint_group);
        npc_controller.move_Speed = speed_Npc;
    }

    private Vector3 RandomPointOnNavMesh(Vector3 center, int index)
    {
        Vector2 random_postion = Random.insideUnitCircle;
        Vector3 save_random_point = new Vector3(random_postion.x, 0, random_postion.y);
        Vector3 random_point_on_circle = center + (spawn_Range * save_random_point);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(random_point_on_circle, out hit, seekRange, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else if (index < 10)
        {
            RandomPointOnNavMesh(center, index + 1);
        }
        return Vector3.zero;
    }
}
