using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Build.Reporting;
using UnityEditor.CrashReporting;

static class FrogBuild {
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
    
    [MenuItem("Frog/Perform Android Build (Mono)")]
    [UsedImplicitly]
    public static void PerformAndroidBuildMono() {
        Environment.SetEnvironmentVariable("DEV_BUILD", "1");
        var locationPathName = "Frog-mono.apk";
        if (PerformAndroidBuildInternal(false, false, locationPathName, false)) {
            EditorUtility.RevealInFinder(Path.Combine(Environment.CurrentDirectory, locationPathName));
        }
    }
    
    [MenuItem("Frog/Perform Android Build (IL2CPP)")]
    [UsedImplicitly]
    public static void PerformAndroidBuildIl2Cpp() {
        Environment.SetEnvironmentVariable("DEV_BUILD", "1");
        Environment.SetEnvironmentVariable("SKIP_ARMV7", "0");
        var locationPathName = "Frog-il2cpp.apk";
        if (PerformAndroidBuildInternal(true, false, locationPathName, false)) {
            EditorUtility.RevealInFinder(Path.Combine(Environment.CurrentDirectory, locationPathName));
        }
    }

    [UsedImplicitly]
    public static void PerformAndroidBuild() {
        var isAppBundle = Environment.GetEnvironmentVariable("ANDROID_APP_BUNDLE") == "1";
        PerformAndroidBuildInternal(true, false, isAppBundle ? "build/Frog.aab" : "build/Frog.apk", isAppBundle);
    }

    static bool PerformAndroidBuildInternal(bool il2Cpp, bool run, string locationPathName, bool isAppBundle) {
        // 자동 빌드 머신에서 ANDROID_NDK_ROOT 환경 변수 인식하도록 하기 위해
        var ndkRootFromEnvVar = Environment.GetEnvironmentVariable("ANDROID_NDK_ROOT");
        if (string.IsNullOrEmpty(ndkRootFromEnvVar) == false)
        {
            Debug.Log($"Setting NDK root from env var... {ndkRootFromEnvVar}");
            EditorPrefs.SetString("AndroidNdkRoot", ndkRootFromEnvVar);
        }
        
        var isReleaseBuild = Environment.GetEnvironmentVariable("DEV_BUILD") != "1";
        var skipArmV7 = Environment.GetEnvironmentVariable("SKIP_ARMV7") == "1";
        
        var options = new BuildPlayerOptions {
            scenes = Scenes,
            target = BuildTarget.Android,
            locationPathName = locationPathName
            
        };
        
        if (run) {
            options.options |= BuildOptions.AutoRunPlayer;
        }

        // Split APKs 옵션 켠다. 개발중에는 끄고 싶을 때도 있을 것
        if (il2Cpp) {
            // 자동 빌드는 IL2CPP로 
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            // Split APK
            //PlayerSettings.Android.buildApkPerCpuArchitecture = true;
            // 개발중일때는 ARM64만 빌드하자. 빠르니까...
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            if (skipArmV7 == false) {
                PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7;
            }
        } else {
            // 개발 기기에서 바로 보고 싶을 땐 mono로 보자
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            // Split Apk 필요 없다
            //PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            // 개발중일때는 ARM64만 빌드하자. 빠르니까...
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }
        
        EditorUserBuildSettings.buildAppBundle = isAppBundle;
        
        // Pro 버전일때만 되는 기능이긴 한데, 이걸 켜고 푸시한 경우도 있을테니 여기서 꺼서 안전장치로 작동하게 한다.
        PlayerSettings.SplashScreen.show = false;
        // 디버그 관련 심볼을 빼서 디버그 메시지 나오지 않도록 한다.
        if (isReleaseBuild) {
            RemovingFrogDebugDefine(BuildTargetGroup.Android);

            // Unity Cloud Diagnostics 활성화한다.
            // 실서비스 빌드는 리포트 켜서 활용한다.
            CrashReportingSettings.enabled = true;
        } else {
            // 개발중 버전 크래시는 리포트 굳이 받을 필요 없다.
            CrashReportingSettings.enabled = false;
        }
        
        Debug.Log($"Setting CrashReportingSettings.enabled to '{CrashReportingSettings.enabled}'");

        var cmdArgs = Environment.GetCommandLineArgs().ToList();
        if (ProcessAndroidKeystorePassArg(cmdArgs)) {
            ProcessBuildNumber(cmdArgs);
            var buildReport = BuildPipeline.BuildPlayer(options);
            if (buildReport.summary.result != BuildResult.Succeeded) {
                if (Application.isBatchMode) {
                    EditorApplication.Exit(-1);
                }

                return false;
            }

            // 빌드 성공!
            return true;
        }

        // 키가 없어서 실패!
        return false;
    }

    static bool ProcessAndroidKeystorePassArg(List<string> cmdArgs) {
        // 이미 채워져있다면 더 할 게 없다.
        if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass) == false
            && string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass) == false) {
            return true;
        }

        var keystorePassIdx = cmdArgs.FindIndex(e => e == "-keystorePass");
        string keystorePass;
        if (keystorePassIdx >= 0) {
            keystorePass = cmdArgs[keystorePassIdx + 1];
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasPass = keystorePass;
            return true;
        } else {
            keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
            if (string.IsNullOrEmpty(keystorePass)) {
                try {
                    keystorePass = File.ReadAllText(".keystore_pass").Trim();
                    PlayerSettings.Android.keystorePass = keystorePass;
                    PlayerSettings.Android.keyaliasPass = keystorePass;
                    return true;
                } catch {
                    Debug.LogError(
                        "-keystorePass argument or '.keystore_pass' file should be provided to build Android APK.");
                }
            } else {
                PlayerSettings.Android.keystorePass = keystorePass;
                PlayerSettings.Android.keyaliasPass = keystorePass;
                return true;
            }
        }

        return false;
    }

    public static AppMetaInfo ProcessBuildNumber(List<string> cmdArgs) {
        var buildNumberIdx = cmdArgs.FindIndex(e => e == "-buildNumber");
        var buildNumber = AppMetaInfo.TemporaryBuildNumber;
        if (buildNumberIdx >= 0) {
            buildNumber = cmdArgs[buildNumberIdx + 1];
        }

        var appMetaInfo = AppMetaInfoEditor.CreateAppMetaInfoAsset();
        appMetaInfo.buildNumber = buildNumber;
        appMetaInfo.buildStartDateTime = DateTime.Now.ToString("MM-dd HH:mm");
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

        var isReleaseBuild = Environment.GetEnvironmentVariable("DEV_BUILD") != "1";
        
        EditorUtility.SetDirty(appMetaInfo);
        AssetDatabase.SaveAssets();
        return appMetaInfo;
    }

    [UsedImplicitly]
    public static void PerformIosBuild() {
        var isReleaseBuild = Environment.GetEnvironmentVariable("DEV_BUILD") != "1";
        PerformIosDistributionBuild(Environment.GetEnvironmentVariable("IOS_PROFILE_GUID"));
    }

    static void PerformIosDistributionBuild(string profileId) {
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = Scenes, target = BuildTarget.iOS, locationPathName = "./build"
        };
        // Pro 버전일때만 되는 기능이긴 한데, 이걸 켜고 푸시한 경우도 있을테니 여기서 꺼서 안전장치로 작동하게 한다.
        PlayerSettings.SplashScreen.show = false;
        // 디버그 관련 심볼을 빼서 디버그 메시지 나오지 않도록 한다.
        if (Environment.GetEnvironmentVariable("DEV_BUILD") != "1") {
            RemovingFrogDebugDefine(BuildTargetGroup.iOS);
            
            // Unity Cloud Diagnostics 활성화한다.
            // 실서비스 빌드는 리포트 켜서 활용한다.
            CrashReportingSettings.enabled = true;
        } else {
            // 개발중 버전 크래시는 리포트 굳이 받을 필요 없다.
            CrashReportingSettings.enabled = false;   
        }
        
        Debug.Log($"Setting CrashReportingSettings.enabled to '{CrashReportingSettings.enabled}'");

        PlayerSettings.iOS.appleDeveloperTeamID = Environment.GetEnvironmentVariable("IOS_TEAM_ID");
        if (string.IsNullOrEmpty(profileId)) {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        } else {
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;
            PlayerSettings.iOS.iOSManualProvisioningProfileID = profileId;
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
        }

        var cmdArgs = Environment.GetCommandLineArgs().ToList();
        ProcessBuildNumber(cmdArgs);
        var buildReport = BuildPipeline.BuildPlayer(options);
        if (buildReport.summary.result != BuildResult.Succeeded && Application.isBatchMode) {
            EditorApplication.Exit(-1);
        }
    }

    static void RemovingFrogDebugDefine(BuildTargetGroup buildTargetGroup) {
        var symbolsToBeRemovedList = new List<string> {
            "DEV_BUILD",
            "CONDITIONAL_DEBUG",
        };
        var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        scriptingDefineSymbols = string.Join(";",
            scriptingDefineSymbols.Split(';').Where(e => symbolsToBeRemovedList.Contains(e) == false));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefineSymbols);
    }
}