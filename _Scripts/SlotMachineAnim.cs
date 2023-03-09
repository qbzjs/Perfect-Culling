using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineAnim : MonoBehaviour
{
    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null) _animator= GetComponent<Animator>();
            return _animator;
        }
    }
    private void Start()
    {
        animator.SetInteger("AnimStop", 1);
    }
    void RandomRoll()
    {
        int value = Random.Range(1, 5);
        animator.SetInteger("AnimStop", value);
    }

}
