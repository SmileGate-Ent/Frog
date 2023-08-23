using System;
using UnityEditor;
using UnityEngine;

public static class PublisherUtil
{
    [MenuItem("Frog/Create Screenshot")]
    static void CreateScreenshot()
    {
        var s = DateTime.Now.ToString("yy-MM-dd HH.mm.ss");
        var fileName = $"Screenshot {s}.png";
        ScreenCapture.CaptureScreenshot(fileName);
        Debug.Log($"Screenshot {fileName} created.");
    }
}