using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoneComponent : MonoBehaviour
{
    public Transform head, shirt, hand, pant, shoes, vehicle;
    [SerializeField] private GameObject _obModelMesh;
    public GameObject obModelMesh => _obModelMesh;
}
