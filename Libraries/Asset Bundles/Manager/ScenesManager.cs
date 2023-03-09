using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : TPRLSingleton<ScenesManager>
{
    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    public AsyncOperation async_temp { get; set; }

    public AsyncOperation GetScene(string bundle_name, string asset_name, bool is_additive, UnityAction action, bool is_allow_activation = false)
    {
        return AssetBundleDownloader.GetScene(bundle_name, asset_name, is_additive, action, is_allow_activation);
    }

    public void UnLoadAllScene()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        int scene_count = SceneManager.sceneCount;
        for (int i = 0; i < scene_count; i++)
        {
            string scene_name = SceneManager.GetSceneAt(i).name;
            if (!scene_name.Equals(activeScene))
            {
                SceneManager.UnloadSceneAsync(scene_name);
            }
        }
        SceneManager.UnloadSceneAsync(activeScene);
    }

    public void UnLoadAllOtherScene(string currentScene, string scene2 = "", UnityAction callback = null)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene));
        int scene_count = SceneManager.sceneCount;
        for (int i = 0; i < scene_count; i++)
        {
            string scene_name = SceneManager.GetSceneAt(i).name;
            if (!scene_name.Equals(currentScene) && !scene_name.Equals(scene2))
            {
                SceneManager.UnloadSceneAsync(scene_name);
            }
        }
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public AsyncOperation UnLoadScene(string currentScene, UnityAction action = null)
    {
        AsyncOperation async = null;
        int scene_count = SceneManager.sceneCount;
        for (int i = 0; i < scene_count; i++)
        {
            string scene_name = SceneManager.GetSceneAt(i).name;
            if (scene_name.Equals(currentScene))
            {
                async = SceneManager.UnloadSceneAsync(scene_name);
                if (action != null)
                    action.Invoke();
                break;
            }
        }
        return async;
    }

    public AsyncOperation UnLoadDuplicateScene(string currentScene, UnityAction action = null)
    {
        int count_same_scene = 0;
        AsyncOperation async = null;
        int scene_count = SceneManager.sceneCount;
        for (int i = 0; i < scene_count; i++)
        {
            string scene_name = SceneManager.GetSceneAt(i).name;
            if (scene_name.Equals(currentScene))
            {
                count_same_scene++;
                if (count_same_scene >= 2)
                {
                    async = SceneManager.UnloadSceneAsync(scene_name);
                    if (action != null)
                        action.Invoke();
                    break;
                }
            }
        }
        return async;
    }
}
