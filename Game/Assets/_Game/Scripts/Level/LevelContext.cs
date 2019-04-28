using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Context", menuName = "Context /Level Context")]
public class LevelContext : ScriptableObject {
  public Level[] Levels;
}
