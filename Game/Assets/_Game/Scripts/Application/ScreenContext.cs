using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Screen Context", menuName = "Context/Screen Context")]
public class ScreenContext : ScriptableObject {
  public List<ScreenController> config;
}
