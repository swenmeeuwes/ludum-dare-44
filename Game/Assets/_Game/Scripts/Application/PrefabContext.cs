using UnityEngine;

[CreateAssetMenu(fileName = "Prefab Context", menuName = "Context/Prefab Context")]
public class PrefabContext : ScriptableObject
{
  public ScreenRoot ScreenRoot;
  public Arrow Arrow;
  public SpawnPoints SpawnPoints;
}
