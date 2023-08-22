using UnityEditor;
using UnityEngine;

public static class PublisherUtil
{
    [MenuItem("Frog/Create Screenshot")]
    static void CreateScreenshot()
    {
        ScreenCapture.CaptureScreenshot("Screenshot.png");    
    }
}
