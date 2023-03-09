using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationEvent : MonoBehaviour
{
    public bool isCollideWithPlayer = false;
    public Transform targetTransform;
    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }
    private NavMeshAgent _agent;
    private NavMeshAgent agent
    {
        get
        {
            if (_agent == null)
            {
                _agent = transform.parent.GetComponent<NavMeshAgent>();
            }
            return _agent;
        }
    }
    private void PlayAnim(string name)
    {
        animator.Play(name);
    }
    private void StandUp()
    {
        SetAnim("isFallAhead", false);
    }
    private void ContinueMoving() {
        agent.isStopped = false;
        animator.Play("Running");
    }
    protected void SetAnim(string name_of_anim_clip, bool active)
    {
        animator.SetBool(name_of_anim_clip, active);
    }
}
