using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    [SerializeField]
    private Button buttonQuitGame;
    // Start is called before the first frame update
    void Start()
    {
        if (buttonQuitGame == null) return;
        buttonQuitGame.onClick.AddListener(OnClickQuitGame);
    }

    // Update is called once per frame
private void OnClickQuitGame()
    {
        Application.Quit();
    }
}
