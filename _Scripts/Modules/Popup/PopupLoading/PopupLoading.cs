using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("PopupLoading", "CanvasFPS")]
public class PopupLoading : BasePanel
{
    private Coroutine corHide;

    private void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void OnEnable()
    {
        if (corHide != null)
            StopCoroutine(corHide);
        corHide = StartCoroutine(IEHide());
    }

    private void OnDisable()
    {
        if (corHide != null)
            StopCoroutine(corHide);
    }

    private void OnDestroy()
    {
        if (corHide != null)
            StopCoroutine(corHide);
    }

    private IEnumerator IEHide()
    {
        yield return new WaitForSeconds(10);
        HidePanel();
    }
}
