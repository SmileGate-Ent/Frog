﻿using UnityEngine;

public class AppMetaInfo : ScriptableObject
{
    public string buildNumber;
    public string buildStartDateTime;
    public int androidBundleVersionCode; // Only for Android
    public string iosBuildNumber; // Only for iOS
    
    public const string TemporaryBuildNumber = "<?>";
    public bool IsTemporaryBuild => buildNumber == TemporaryBuildNumber;
}