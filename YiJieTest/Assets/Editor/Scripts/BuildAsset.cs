using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class AssetBundle : ScriptableObject
{

    [MenuItem("Tools/BuildAsset")]
    static void DoIt()
    {
        DirectoryInfo directory=new DirectoryInfo(Application.dataPath+ "/StreamingAssets");
        if (!directory.Exists)
        {
            directory.Create();
        }
        BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;
        BuildTarget plant = BuildTarget.Android;
        BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath("Assets/Resources/RawImage.prefab"), null, "Assets/StreamingAssets/RawImage.u3d", options, plant);
        AssetDatabase.Refresh();
    }
}
