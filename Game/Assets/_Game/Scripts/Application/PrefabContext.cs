using UnityEngine;

[CreateAssetMenu(fileName = "Prefab Context", menuName = "Context/Prefab Context")]
public class PrefabContext : ScriptableObject
{
  public ScreenRoot ScreenRoot;
  public Arrow Arrow;
  public FlyingEnemySpawnPoints FlyingEnemySpawnPoints;
  public HeartView HeartView;
  public ShopItemView ShopItemView;
  public FallingObstaclesSpawnPoints FallingObstaclesSpawnPoints;
  public SpikeyBall SpikeyBall;
}
