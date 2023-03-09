using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Time;

public class FPS : TPRLSingleton<FPS>
{
    [SerializeField]
    TextMeshProUGUI txFps;
    ObscuredBool is_destroy = false;
    [SerializeField]
    private ObscuredFloat Rate = 120f;
    ObscuredFloat currentFrameTime;

    protected override void Awake()
    {
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        dontDestroyOnLoad = true;
        base.Awake();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;
        currentFrameTime = SpeedHackProofTime.realtimeSinceStartup;
        //Time.captureFramerate = 120;
        //StartCoroutine("WaitForNextFrame");
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev || environment == EnvironmentType.test)
        {
            is_destroy = false;
            //StopAllCoroutines();

            //StartCoroutine(IEShowFPS());
            InvokeRepeating("ShowFPS", 1, 1);
        }
    }

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / Rate;
            ObscuredFloat t = SpeedHackProofTime.realtimeSinceStartup;
            ObscuredFloat sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((ObscuredInt)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = SpeedHackProofTime.realtimeSinceStartup;
        }
    }

    private void ShowFPS()
    {
        ObscuredFloat deltaTime = 0.0f;
        deltaTime += (SpeedHackProofTime.unscaledDeltaTime - deltaTime);
        ObscuredFloat fps = 1.0f / deltaTime;
        if (txFps != null)
            txFps.text = string.Format("Fps : {0:0.0}", fps);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    float deltaTime = 0.0f;
    //    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    //    float fps = 1.0f / deltaTime;
    //    //ShowFPS(fps);
    //}

    //private IEnumerator IEShowFPS()
    //{
    //    while (true)
    //    {
    //        if (is_destroy) break;
    //        float deltaTime = 0.0f;
    //        deltaTime += (Time.unscaledDeltaTime - deltaTime);
    //        float fps = 1.0f / deltaTime;
    //        if (txFps != null)
    //            txFps.text = string.Format("Fps : {0:0.0}", fps);
    //        yield return new WaitForSeconds(1);
    //    }
    //}

    protected override void OnDestroy()
    {
        base.OnDestroy();
        is_destroy = true;
        StopAllCoroutines();
    }

}
