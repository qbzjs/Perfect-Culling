using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseOpenPanel : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { Open(); /*SoundController.instance.OnClickButton();*/  });
    }

    protected virtual void Open() {  }
}
