using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

[UsedImplicitly]
internal class FrogBuild
{
    static string[] Scenes
    {
        get
        {
            return new[]
            {
                "Assets/Scenes/LogoScene.unity",
                "Assets/Scenes/TitleScene.unity",
                "Assets/Scenes/InGame.unity",
            };
        }
    }

    [MenuItem("Frog/Perform Android Build && Run (Mono)")]
    public static void PerformAndroidBuildMono()
    {
        Environment.SetEnvironmentVariable("DEV_BUILD", "1");
        PerformAndroidBuildInternal(false, true);
    }

    [UsedImplicitly]
    public static void PerformAndroidBuild()
    {
        PerformAndroidBuildInternal(true);
    }

    static void PerformAndroidBuildInternal(bool il2Cpp, bool run = false)
    {
        // 자동 빌드 머신에서 ANDROID_NDK_ROOT 환경 변수 인식하도록 하기 위해
        var ndkRootFromEnvVar = Environment.GetEnvironmentVariable("ANDROID_NDK_ROOT");
        if (string.IsNullOrEmpty(ndkRootFromEnvVar) == false)
        {
            Debug.Log($"Setting NDK root from env var... {ndkRootFromEnvVar}");
            EditorPrefs.SetString("AndroidNdkRoot", ndkRootFromEnvVar);
        }

        var isReleaseBuild = Environment.GetEnvironmentVariable("DEV_BUILD") != "1";
        var skipArmV7 = Environment.GetEnvironmentVariable("SKIP_ARMV7") == "1";
        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            target = BuildTarget.Android,
            locationPathName = "./frog.apk"
        };
        if (run) options.options |= BuildOptions.AutoRunPlayer;
        // Split APKs 옵션 켠다. 개발중에는 끄고 싶을 때도 있을 것
        if (il2Cpp)
        {
            // 자동 빌드는 IL2CPP로 
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            // Split APK
            PlayerSettings.Android.buildApkPerCpuArchitecture = true;
            // 개발중일때는 ARM64만 빌드하자. 빠르니까...
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            if (isReleaseBuild || skipArmV7 == false)
                PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7;
        }
        else
        {
            // 개발 기기에서 바로 보고 싶을 땐 mono로 보자
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            // Split Apk
            PlayerSettings.Android.buildApkPerCpuArchitecture = true;
            // 개발중일때는 ARM64만 빌드하자. 빠르니까...
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }

        // Pro 버전일때만 되는 기능이긴 한데, 이걸 켜고 푸시한 경우도 있을테니 여기서 꺼서 안전장치로 작동하게 한다.
        PlayerSettings.SplashScreen.show = false;
        // PCG_DEBUG 심볼을 빼서 디버그 메시지 나오지 않도록 한다.
        if (isReleaseBuild) RemovingPcgDebugDefine(BuildTargetGroup.Android);
        var cmdArgs = Environment.GetCommandLineArgs().ToList();
        if (ProcessAndroidKeystorePassArg(cmdArgs))
        {
            ProcessBuildNumber(cmdArgs);
            var buildReport = BuildPipeline.BuildPlayer(options);
            if (buildReport.summary.result != BuildResult.Succeeded) EditorApplication.Exit(-1);
        }
    }

    static bool ProcessAndroidKeystorePassArg(List<string> cmdArgs)
    {
        var keystorePassIdx = cmdArgs.FindIndex(e => e == "-keystorePass");
        string keystorePass;
        if (keystorePassIdx >= 0)
        {
            keystorePass = cmdArgs[keystorePassIdx + 1];
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasPass = keystorePass;
            return true;
        }

        keystorePass = Environment.GetEnvironmentVariable("PCG_KEYSTORE_PASS");
        if (string.IsNullOrEmpty(keystorePass))
        {
            Debug.LogError("-keystorePass argument should be provided to build Android APK.");
        }
        else
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasPass = keystorePass;
            return true;
        }

        return false;
    }

    public static AppMetaInfo ProcessBuildNumber(List<string> cmdArgs)
    {
        var buildNumberIdx = cmdArgs.FindIndex(e => e == "-buildNumber");
        var buildNumber = "<?>";
        if (buildNumberIdx >= 0) buildNumber = cmdArgs[buildNumberIdx + 1];
        var appMetaInfo = AppMetaInfoEditor.CreateAppMetaInfoAsset();
        appMetaInfo.buildNumber = buildNumber;
        appMetaInfo.buildStartDateTime = DateTime.Now.ToShortDateString();
#if UNITY_ANDROID
        appMetaInfo.androidBundleVersionCode = PlayerSettings.Android.bundleVersionCode;
#else
        appMetaInfo.androidBundleVersionCode = -1;
#endif
#if UNITY_IOS
        appMetaInfo.iosBuildNumber = PlayerSettings.iOS.buildNumber;
#else
        appMetaInfo.iosBuildNumber = "INVALID";
#endif
        EditorUtility.SetDirty(appMetaInfo);
        AssetDatabase.SaveAssets();
        return appMetaInfo;
    }

    [UsedImplicitly]
    public static void PerformIosBuild()
    {
        PerformIosDistributionBuild(Environment.GetEnvironmentVariable("IOS_PROFILE_GUID"));
    }

    static void PerformIosDistributionBuild(string profileId)
    {
        var options = new BuildPlayerOptions { scenes = Scenes, target = BuildTarget.iOS, locationPathName = "./build" };
        // Pro 버전일때만 되는 기능이긴 한데, 이걸 켜고 푸시한 경우도 있을테니 여기서 꺼서 안전장치로 작동하게 한다.
        PlayerSettings.SplashScreen.show = false;
        // PCG_DEBUG 심볼을 빼서 디버그 메시지 나오지 않도록 한다.
        if (Environment.GetEnvironmentVariable("DEV_BUILD") != "1") RemovingPcgDebugDefine(BuildTargetGroup.iOS);

        PlayerSettings.iOS.appleDeveloperTeamID = "TG9MHV97AH";
        if (string.IsNullOrEmpty(profileId))
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        }
        else
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            PlayerSettings.iOS.iOSManualProvisioningProfileID = profileId;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
        }

        var cmdArgs = Environment.GetCommandLineArgs().ToList();
        ProcessBuildNumber(cmdArgs);
        var buildReport = BuildPipeline.BuildPlayer(options);
        if (buildReport.summary.result != BuildResult.Succeeded) EditorApplication.Exit(-1);
    }

    static void RemovingPcgDebugDefine(BuildTargetGroup buildTargetGroup)
    {
        var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        scriptingDefineSymbols = string.Join(";",
            scriptingDefineSymbols.Split(';').Where(e => e != "PCG_DEBUG" && e != "PCG_ADMIN"));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
    }
}
