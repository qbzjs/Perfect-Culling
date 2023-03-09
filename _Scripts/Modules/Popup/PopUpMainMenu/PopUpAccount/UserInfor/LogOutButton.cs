using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogOutButton : MonoBehaviour
{
    [SerializeField] private Button btLogout;
    // Start is called before the first frame update
    void Start()
    {
        if (btLogout != null)
            btLogout.onClick.AddListener(ClickLogout);
    }
    private void ClickLogout()
    {
        TPRLSoundManager.Instance.StopMusic();
        ObscuredPrefs.DeleteAll();
        StopAllCoroutines();
        InputRegisterEvent.Instance.ClearAllEvents();
        TPRLSoundManager.Instance.ClearAllSoundOfVideo();
        ScenesManager.Instance.GetScene(BundleName.SCENES, AllSceneName.Login, false, null, true);

    }

}
