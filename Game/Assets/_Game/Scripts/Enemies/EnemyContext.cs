using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Context", menuName = "Context/Enemy Context")]
public class EnemyContext : ScriptableObject {
  public Enemy[] Enemies;
}