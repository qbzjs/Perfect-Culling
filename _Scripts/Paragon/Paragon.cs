using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paragon : MonoBehaviour
{

    public string paragon_id;
    public string paragon_name;
    public float paragon_power;
    public float paragon_price;
    private void Start()
    {
        transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
    }
}
