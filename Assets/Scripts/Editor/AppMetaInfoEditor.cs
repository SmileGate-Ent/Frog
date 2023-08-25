using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public class AppMetaInfoEditor
{
    [MenuItem("Assets/Create/App Meta Info")]
    public static void CreateAppMetaInfoAssetMenu()
    {
        var asset = FrogBuild.ProcessBuildNumber(new List<string>());
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public static AppMetaInfo CreateAppMetaInfoAsset()
    {
        var appMetaInfo = ScriptableObject.CreateInstance<AppMetaInfo>();
        AssetDatabase.CreateAsset(appMetaInfo, "Assets/Resources/App Meta Info.asset");
        EditorUtility.SetDirty(appMetaInfo);
        AssetDatabase.SaveAssets();
        appMetaInfo.buildNumber = "123";
        appMetaInfo.buildStartDateTime = System.DateTime.Now.ToShortDateString();
        return appMetaInfo;
    }
}
