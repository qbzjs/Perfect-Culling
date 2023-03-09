using UnityEditor;
using UnityEngine;

public class AssetBundlesMenu
{
	[MenuItem("Hai/AssetBundles/Clear All")]
	static public void AssetBundleClearAll()
	{
		PlayerPrefs.DeleteAll();
		Caching.ClearCache();
	}

	[MenuItem("Hai/AssetBundles/Clear AssetBundles")]
	static public void AssetBundleClearCache()
	{
		Caching.ClearCache();
	}

	const string kSimulationMode = "Hai/AssetBundles/Simulation Mode";

	[MenuItem(kSimulationMode)]
	public static void ToggleSimulationMode()
	{
		AssetBundleDownloader.IsSimulateAssetBundleInEditor = !AssetBundleDownloader.IsSimulateAssetBundleInEditor;
	}

	[MenuItem(kSimulationMode, true)]
	public static bool ToggleSimulationModeValidate()
	{
		Menu.SetChecked(kSimulationMode, AssetBundleDownloader.IsSimulateAssetBundleInEditor);
		return true;
	}

	[MenuItem("Hai/AssetBundles/Build AssetBundles #&b")]
	static public void Build()
	{
		if (UnityEditor.EditorUtility.DisplayDialog("Confirm", "Do you want to build asset bundle for " + EditorUserBuildSettings.activeBuildTarget.ToString() + "?", "Yes", "No"))
		{
            BuildAssetBundles.Build();
		}
	}

	[MenuItem("Hai/AssetBundles/Build Asset Android")]
	static public void BuildAssetAndroid()
	{
		if (UnityEditor.EditorUtility.DisplayDialog("Confirm", "Do you want to build asset bundle for Android?", "Yes", "No"))
		{
			BuildAssetBundles.Build();
		}
	}

	[MenuItem("Hai/AssetBundles/Build Asset iOS")]
	static public void BuildAssetiOS()
	{
		if (UnityEditor.EditorUtility.DisplayDialog("Confirm", "Do you want to build asset bundle for iOS?", "Yes", "No"))
		{
			BuildAssetBundles.Build();
		}
	}
}
