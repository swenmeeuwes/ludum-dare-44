using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AsepriteExporter {
  public readonly static string AsepritePath = @"C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe"; // Path to Aseprite executable
  public readonly static string ExportAtRelativePath = @"Export"; // Files will be exported to this folder (e.g. `Assets/player.aseprite` -> `Assets/Export/player.png`

  [MenuItem("Assets/Aseprite/Export Selection To PNG")]
  private static void ExportAsepriteFilesToPng() {
    var selectedAsepriteFilePaths = GetAsepriteFilePaths(Selection.objects);
    
    var asepriteFiles = selectedAsepriteFilePaths
      .Select((f) => Path.GetFileName(f))
      .OrderBy((a) => a)
      .Aggregate((a, b) => a + "\n- " + b);

    var hasConfirmed = EditorUtility.DisplayDialog(
      "Aseprite export",
      string.Format("Are you sure you want to export the aseprite files:\n- {0}", asepriteFiles),
      "Ok",
      "Cancel");

    if (!hasConfirmed) {
      return;
    }

    foreach (var selectedAsepriteFilePath in selectedAsepriteFilePaths) {
      var sourceAsepriteFullFilePath = Path.GetFullPath(selectedAsepriteFilePath);
      var targetFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceAsepriteFullFilePath);
      var sourceAsepriteDirectory = Path.GetDirectoryName(sourceAsepriteFullFilePath);
      var targetFilePath = Path.Combine(sourceAsepriteDirectory, ExportAtRelativePath, targetFileNameWithoutExtension) + ".png";

      var processStartInfo = new ProcessStartInfo(AsepritePath) {
        Arguments = string.Format("--batch {0} --save-as {1}", sourceAsepriteFullFilePath, targetFilePath),
        UseShellExecute = true,
        CreateNoWindow = false
      };

      var process = Process.Start(processStartInfo);
      process.WaitForExit();
      UnityEngine.Debug.LogFormat("[Aseprite Exporter] Exported {0} in {1}", Path.GetFileName(selectedAsepriteFilePath), selectedAsepriteFilePath);
    }

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
  }

  private static IEnumerable<string> GetAsepriteFilePaths(Object[] objects) {
    return objects
     .Select(o => AssetDatabase.GetAssetPath(o))
     .Where(IsAsepriteFile);
  }

  private static bool IsAsepriteFile(string filePath) {
    var extension = Path.GetExtension(filePath).Trim('.').ToLower();
    return extension == "ase" || extension == "aseprite";
  }
}
