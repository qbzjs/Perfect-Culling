using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGroup : MonoBehaviour
{
    public List<Transform> lst_WayPoint;
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Transform item in transform)
        {
            lst_WayPoint.Add(item);
        }
    }
}
