using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

public class IosPostProcessBuild : IPostprocessBuildWithReport
{
    public int callbackOrder => 999;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
#if UNITY_IOS
            var projectPath = Path.Combine(report.summary.outputPath, "Unity-iPhone.xcodeproj", "project.pbxproj");

            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            // Facebook SDK가 Bitcode 미지원하므로 이 플래그를 꺼야 빌드가 된다.
            //string target = pbxProject.TargetGuidByName("Unity-iPhone");
            var target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            // 로컬 알림 관련해서 아래 프레임워크가 추가 되어야 한다.
            pbxProject.AddFrameworkToProject(target, "UserNotifications.framework", false);

            pbxProject.AddCapability(target, PBXCapabilityType.iCloud);
            pbxProject.AddCapability(target, PBXCapabilityType.GameCenter);
            pbxProject.AddCapability(target, PBXCapabilityType.InAppPurchase);
            // xml2: Facebook Audience Network에서 필요로 한다.
            pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-lxml2");

            var unityFrameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO");
            // z: gRPC에서 필요로 한다.
            pbxProject.AddBuildProperty(unityFrameworkTarget, "OTHER_LDFLAGS", "-lz");

            pbxProject.WriteToFile(projectPath);

            var plistPath = Path.Combine(report.summary.outputPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // 수출 관련 규정 플래그 추가 (AppStore 제출 시 필요하다고 안내하고 있음)
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            // 스크린샷을 앨범에 저장하고자 할 때 필요한 권한을 요청하는 팝업 설정 (지정하지 않으면 크래시)
            //plist.root.SetString("NSPhotoLibraryUsageDescription", "Screenshot Save");
            //plist.root.SetString("NSPhotoLibraryAddUsageDescription", "Screenshot Save");

            // https://developers.google.com/ad-manager/mobile-ads-sdk/ios/quick-start#update_your_infoplist
            //plist.root.SetBoolean("GADIsAdManagerApp", true);

            // ERROR ITMS-90503: "Invalid Bundle. You've included the "arm64" value for the UIRequiredDeviceCapabilities key in your Xcode project, ...
            // var devCapArray = plist.root["UIRequiredDeviceCapabilities"].AsArray();
            // devCapArray.values = devCapArray.values.Where(e => e.AsString() != "arm64").ToList();
            // plist.root["UIRequiredDeviceCapabilities"] = devCapArray;

            plist.WriteToFile(plistPath);

            // Copy entitlements file
            File.Copy("frog.entitlements", Path.Combine(report.summary.outputPath, "frog.entitlements"), true);
#endif
        }
    }
}