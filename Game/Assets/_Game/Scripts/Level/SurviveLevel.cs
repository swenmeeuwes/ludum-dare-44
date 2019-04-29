using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Survive Level", menuName = "Levels/Survive Level")]
public class SurviveLevel : Level {
  public EnemyModel[] Enemies;
  public bool WithFallingObstacles;
  public float FallingObstacleOffset; // in seconds
  public float FallingObstacleInterval; // in seconds

  [HideInInspector] public Dictionary<Enemy, int> SpawnedEnemies;
  [HideInInspector] public Dictionary<Enemy, int> KilledEnemies;

  public void InitializeLogging() {
    SpawnedEnemies = new Dictionary<Enemy, int>();
    KilledEnemies = new Dictionary<Enemy, int>();

    foreach (var configuredEnemyModel in Enemies) {
      SpawnedEnemies.Add(configuredEnemyModel.Enemy, 0);
      KilledEnemies.Add(configuredEnemyModel.Enemy, 0);
    }
  }

  public void LogSpawn(Enemy enemy) {
    var key = SpawnedEnemies.First(s => enemy.GetType() == s.Key.GetType()).Key;
    SpawnedEnemies[key]++;
  }

  public void LogKill(Enemy enemy) {
    var key = KilledEnemies.First(k => enemy.GetType() == k.Key.GetType()).Key;
    KilledEnemies[key]++;
  }

  public bool IsFinished {
    get {
      var allEnemiesAreKilled = KilledEnemies.All(k => Enemies.First(e => e.Enemy.GetType() == k.Key.GetType()).Amount == k.Value);

      return AllEnemiesAreSpawned && allEnemiesAreKilled;
    }
  }

  public bool AllEnemiesAreSpawned {
    get {
      return SpawnedEnemies.All(s => Enemies.First(e => e.Enemy.GetType() == s.Key.GetType()).Amount == s.Value);
    }
  }

  public EnemyModel GetRandomEnemyModel() {
    var availableEnemies = Enemies.Where(e => SpawnedEnemies[e.Enemy] < e.Amount).ToArray();
    return availableEnemies[UnityEngine.Random.Range(0, availableEnemies.Length)];
  }

  [Serializable]
  public class EnemyModel {
    public Enemy Enemy;
    public int Amount;
    public float SpawnCooldownAfterSpawnedInSeconds;
  }
}
