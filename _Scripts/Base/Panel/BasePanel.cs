using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected virtual void Awake()
    {
        //PanelManager.Register(this);
    }

    public virtual void HidePanel()
    {
        PanelManager.Hide(this);
    }
}
