using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[UIPanelPrefabAttr("PopUpCompleteQuestEffect", "PopupCanvas")]
public class PopUpCompleteQuestEffect : BasePanel
{
    [SerializeField] private TMP_Text completeText;
    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
    }
    public void PlayAnimComplete(bool is_complete)
    {
        if (is_complete)
        {
            completeText.text = "Quest Completed!";
            animator.SetTrigger("complete");
        }
        else {
            completeText.text = "Quest Failed";
            animator.SetTrigger("fail");
        }
    }
}
