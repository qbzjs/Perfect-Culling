using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Button bt_ResetAll;

    [SerializeField] private SoundSetting ob_Sound;
    [SerializeField] private QualitySetting ob_Quality;

    // Start is called before the first frame update
    void Start()
    {
        if (bt_ResetAll == null) return;
        bt_ResetAll.onClick.AddListener(OnClickBtResetAll);
    }

    // Update is called once per frame
    private void OnClickBtResetAll()
    {
        ob_Sound.DefaultSettingSound();
        ob_Quality.DefaultQuality();

    }
}
