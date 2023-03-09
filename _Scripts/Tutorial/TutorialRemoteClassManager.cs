using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("PopUpTutoriaLClass","Canvas")] 
public class TutorialRemoteClassManager : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }


}
