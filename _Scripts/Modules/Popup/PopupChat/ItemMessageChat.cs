using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TMPro;
using UnityEngine;

public class ItemMessageChat : MonoBehaviour
{
    [SerializeField] TMP_Text text;


    // Start is called before the first frame update
    public void SetInfor(RecordChat chats,bool isMine)
    {
        if (text != null)
        {
            StringBuilder message = new StringBuilder();
            if (!isMine)
            {
                message.Append("<color=#FFB935>");
            }
            else
            {
                message.Append("<color=#4BA8FF>");
            }
            message.Append(chats.s_id);
            message.Append("</color>");
            message.Append(": ");
            message.Append(chats.m);
            text.text = message.ToString();

        }
    }
}