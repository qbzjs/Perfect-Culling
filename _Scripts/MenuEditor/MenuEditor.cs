using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using Common;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;

public class MenuEditor : MonoBehaviour
{
#if UNITY_EDITOR
    const string testMode = "Hai/Build/Test Mode";
    //private static bool isTestMode = false;
    private static string kisTestMode = "TestModeInEditor";
    [MenuItem(testMode)]
    public static void ToggleTestMode()
    {
        bool isTestMode = PlayerPrefs.GetInt("isTestMode", 0) == 1 ? true : false;
        isTestMode = !isTestMode;
        PlayerPrefs.SetInt("isTestMode", isTestMode ? 1 : 0);
        PlayerPrefs.Save();
        EditorPrefs.SetBool(kisTestMode, isTestMode);
        TestMode();
    }

    [MenuItem(testMode, true)]
    public static bool ToggleTestModeValidate()
    {
        Menu.SetChecked(testMode, PlayerPrefs.GetInt("isTestMode", 0) == 1);
        return true;
    }
    private static void TestMode()
    {
        string definesSymbol = PlayerPrefs.GetInt("isTestMode", 0) == 1 ? "TEST_MODE, TEST_TOWN, DEV" : "";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, definesSymbol);
    }

    const string buildAPK = "Hai/Build/Build APK";
    //private static bool isBuildAPK = false;
    private static string kisBuildAPK = "BuildAPKInEditor";
    [MenuItem(buildAPK)]
    public static void ToggleBuildAPK()
    {
        bool isBuildAPK = PlayerPrefs.GetInt("isBuildAPK", 0) == 1 ? true : false;
        isBuildAPK = !isBuildAPK;
        PlayerPrefs.SetInt("isTestMode", isBuildAPK ? 1 : 0);
        PlayerPrefs.SetInt("isBuildAPK", isBuildAPK ? 1 : 0);
        PlayerPrefs.Save();
        EditorPrefs.SetBool(kisBuildAPK, isBuildAPK);
        BuildAPK();
    }

    [MenuItem(buildAPK, true)]
    public static bool ToggleBuildAPKValidate()
    {
        Menu.SetChecked(buildAPK, PlayerPrefs.GetInt("isBuildAPK", 0) == 1);
        return true;
    }
    private static void BuildAPK()
    {
        bool isBuildAPK = PlayerPrefs.GetInt("isBuildAPK", 0) == 1;
        string definesSymbol = isBuildAPK ? "TEST_MODE, TEST_TOWN, DEV, BUILD_APK" : "";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, definesSymbol);
        string source = Application.dataPath;
        string streaming_assets = Path.Combine(Application.streamingAssetsPath, $"AssetBundlesOffline/Android");
        if (File.Exists(streaming_assets) == false)
            Directory.CreateDirectory(streaming_assets);
        if (isBuildAPK)
        {
            source = source.Substring(0, source.LastIndexOf("/") + 1);
            CopyFilesRecursively(source + "AssetBundles/Android", streaming_assets);
        }
        else
        {
            foreach (string newPath in Directory.GetFiles(streaming_assets, "*.*", SearchOption.AllDirectories))
            {
                File.Delete(newPath);
            }
        }
    }

    const string buildiOS = "Hai/Build/Build iOS";
    //private static bool isBuildiOS = false;
    private static string kisBuildiOS = "BuildiOSInEditor";
    [MenuItem(buildiOS)]
    public static void ToggleBuildiOS()
    {
        bool isBuildiOS = PlayerPrefs.GetInt("isBuildiOS", 0) == 1 ? true : false;
        isBuildiOS = !isBuildiOS;
        PlayerPrefs.SetInt("isBuildiOS", isBuildiOS ? 1 : 0);
        EditorPrefs.SetBool(kisBuildiOS, isBuildiOS);
        BuildiOS();
    }

    [MenuItem(buildiOS, true)]
    public static bool ToggleBuildiOSValidate()
    {
        Menu.SetChecked(buildiOS, PlayerPrefs.GetInt("isBuildiOS", 0) == 1);
        return true;
    }
    private static void BuildiOS()
    {
        bool isBuildiOS = PlayerPrefs.GetInt("isBuildiOS", 0) == 1;
        string source = Application.dataPath;
        string streaming_assets = Path.Combine(Application.streamingAssetsPath, $"AssetBundlesOffline/iOS");
        if (File.Exists(streaming_assets) == false)
            Directory.CreateDirectory(streaming_assets);
        if (isBuildiOS)
        {
            source = source.Substring(0, source.LastIndexOf("/") + 1);
            CopyFilesRecursively(source + "AssetBundles/iOS", streaming_assets);
        }
        else
        {
            foreach (string newPath in Directory.GetFiles(streaming_assets, "*.*", SearchOption.AllDirectories))
            {
                File.Delete(newPath);
            }
        }
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }


    [MenuItem("Hai/Play")]
    private static void Play()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Splash.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Hai/Hack Currency")]
    private static void HackCurrency()
    {
        //SetWave50();
        //UserDatas.userCurrencyInfo.Gem += 999999999;
        //UserDatas.userCurrencyInfo.Gold += 999999999;
    }

    [MenuItem("Hai/Hack Currency 0")]
    private static void HackCurrency0()
    {
        //SetWave50();
        //UserDatas.userCurrencyInfo.Gem = 0;
        //UserDatas.userCurrencyInfo.Gold = 0;
    }

    [MenuItem("Hai/Splash")]
    private static void Splash()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Splash.unity");
    }

    [MenuItem("Hai/Loading")]
    private static void Loading()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Loading.unity");
    }

    [MenuItem("Hai/GamePlayCanvas")]
    private static void GamePlayCanvas()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/GamePlayCanvas.unity");
    }
    [MenuItem("Hai/CreateCharacter")]
    private static void CreateCharacter()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/CreateCharacter.unity");
    }
    [MenuItem("Hai/GamePlayMain")]
    private static void GamePlayMain()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/GamePlayMain.unity");
    }

    [MenuItem("Hai/Login")]
    private static void Login()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/Login.unity");
    }

    [MenuItem("Hai/Reset game data")]
    private static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Hai/ClassRoomCanvas")]
    private static void ClassRoomCanvas()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/ClassRoom/ClassRoomCanvas.unity");
    }

    [MenuItem("Hai/ClassRoom")]
    private static void ClassRoom()
    {
        EditorSceneManager.OpenScene("Assets/_DataAddressable/Scenes/ClassRoom/ClassRoom.unity");
    }

#endif
}
