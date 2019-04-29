using UnityEditor;
using UnityEngine;

public class PlayerPreferenceTool {
  [MenuItem("Tools/Player Preferences/Clear")]
  private static void ClearPlayerPrefences() {
    PlayerPrefs.DeleteAll();
  }
}
