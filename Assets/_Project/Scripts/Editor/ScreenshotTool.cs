using System.IO;
using UnityEditor;
using UnityEngine;

namespace VertigoCase.Editor
{
    /// <summary>
    /// Captures a Game view screenshot for the submission deliverables.
    /// </summary>
    public static class ScreenshotTool
    {
        private const string Folder = "Screenshots";

        [MenuItem("VertigoCase/Capture Screenshot")]
        public static void Capture()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            string filename = $"{Screen.width}x{Screen.height}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            string path = Path.Combine(Folder, filename);

            ScreenCapture.CaptureScreenshot(path);
            Debug.Log($"Screenshot saved: {path}");
        }
    }
}
