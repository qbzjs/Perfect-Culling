using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextPopUpInteract : MonoBehaviour
{
    [SerializeField] private TMP_Text interactTypeText;
    public Image imageClick;
    // Start is called before the first frame update
    public void SetInteractTypeText(string type)
    {
        if (interactTypeText != null)
        {
            interactTypeText.text = type;
        }
    }
}
