using AssetBundles;
using System.IO;
using UnityEditor;

public class BuildAssetBundles
{
    public static void Build()
    {
        string directory = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetAssetBundleNameWithoutVersion());
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}
