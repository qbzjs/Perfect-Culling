using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace AssetBundles
{
    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetAssetBundleName()
        {

#if UNITY_EDITOR
#if OBFUSCATOR
            return $"{ GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget)}_{ getVersionBundle()}_OBFUSCATOR";
#else
            return $"{GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget)}_{getVersionBundle()}";
#endif
#else
#if OBFUSCATOR
            return $"{GetPlatformForAssetBundles(Application.platform)}_{getVersionBundle()}_OBFUSCATOR";
#else
            return $"{GetPlatformForAssetBundles(Application.platform)}_{getVersionBundle()}";
#endif
#endif
        }

        public static string GetAssetBundleNameWithoutVersion()
        {

#if UNITY_EDITOR
#if OBFUSCATOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget) + "_OBFUSCATOR";
#else
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#endif
#else
#if OBFUSCATOR
            return GetPlatformForAssetBundles(Application.platform) + "_OBFUSCATOR";
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
#endif
        }

        public static string GetAssetAndroid()
        {
            return getVersionBundle();
        }
        public static string GetAssetiOS()
        {
            return getVersionBundle();
        }

        public static string getVersionBundle()
        {
            return "3";
        }


#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
    }
}