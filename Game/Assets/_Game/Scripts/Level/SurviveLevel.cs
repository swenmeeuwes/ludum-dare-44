using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Survive Level", menuName = "Levels/Survive Level")]
public class SurviveLevel : Level {
  public EnemyModel[] Enemies;
  public float EnemySpawnOffset; // in seconds

  public bool WithFallingObstacles;
  public int ConcurrentFallingObstacles = 1;
  public float FallingObstacleOffset; // in seconds
  public float FallingObstacleInterval; // in seconds

  public float PlatformActionDelay; // in seconds
  public int AvailablePlatforms = 4;

  public int Allies = 0;
  public float AllySpawnOffset = 0;
  public float AllySpawnInterval = 0;


  [HideInInspector] public Dictionary<Enemy, int> SpawnedEnemies;
  [HideInInspector] public Dictionary<Enemy, int> KilledEnemies;
  [HideInInspector] public int SpawnedAllies = 0;
  [HideInInspector] public List<FlyingHeart> AliveAllies = new List<FlyingHeart>();

  public void InitializeLogging() {
    SpawnedEnemies = new Dictionary<Enemy, int>();
    KilledEnemies = new Dictionary<Enemy, int>();

    foreach (var configuredEnemyModel in Enemies) {
      SpawnedEnemies.Add(configuredEnemyModel.Enemy, 0);
      KilledEnemies.Add(configuredEnemyModel.Enemy, 0);
    }
  }

  public void LogEnemySpawn(Enemy enemy) {
    var key = SpawnedEnemies.First(s => enemy.GetType() == s.Key.GetType()).Key;
    SpawnedEnemies[key]++;
  }

  public void LogKill(Enemy enemy) {
    var key = KilledEnemies.First(k => enemy.GetType() == k.Key.GetType()).Key;
    KilledEnemies[key]++;
  }

  public void LogAllySpawn(FlyingHeart ally) {
    SpawnedAllies++;
    AliveAllies.Add(ally);
  }

  public int AlliesToSpawnLeft { get => Allies - SpawnedAllies; }

  public bool IsFinished {
    get {
      var allEnemiesAreKilled = KilledEnemies.All(k => Enemies.First(e => e.Enemy.GetType() == k.Key.GetType()).Amount == k.Value);

      return AllEnemiesAreSpawned && allEnemiesAreKilled && AllAlliesAreSpawned && AllAlliesAreDead;
    }
  }

  public bool AllEnemiesAreSpawned {
    get {
      return SpawnedEnemies.All(s => Enemies.First(e => e.Enemy.GetType() == s.Key.GetType()).Amount == s.Value);
    }
  }

  public bool AllAlliesAreSpawned {
    get {
      return SpawnedAllies >= Allies; // todo fix >=, we need this now because we set Allies to 0 when spawning them all at once in SurviveLevelHandler...
    }
  }

  public bool AllAlliesAreDead {
    get {
      return AliveAllies.All(a => a == null);
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
