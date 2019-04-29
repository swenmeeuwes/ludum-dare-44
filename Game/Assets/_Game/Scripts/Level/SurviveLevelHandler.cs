using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SurviveLevelHandler : LevelHandler, IInitializable, IDisposable {
  private SignalBus _signalBus;
  private EnemyFactory _enemyFactory;
  private FlyingEnemySpawnPoints _flyingEnemySpawnPoints;
  private FallingObstacleManager _fallingObstacleManager;
  private PlatformManager _platformManager;
  private FlyingHeartFactory _flyingHeartFactory;

  private SurviveLevel _level;
  private float _lastEnemySpawnTime;
  private float _lastObstacleSpawnTime;
  private float _lastAllySpawnTime;

  private SurviveLevel.EnemyModel _lastSpawned;

  [Inject]
  private void Construct(SignalBus signalBus, EnemyFactory enemyFactory, FlyingEnemySpawnPoints flyingEnemySpawnPoints, 
    FallingObstacleManager fallingObstacleManager, PlatformManager platformManager, FlyingHeartFactory flyingHeartFactory) {
    _signalBus = signalBus;
    _enemyFactory = enemyFactory;
    _flyingEnemySpawnPoints = flyingEnemySpawnPoints;
    _fallingObstacleManager = fallingObstacleManager;
    _platformManager = platformManager;
    _flyingHeartFactory = flyingHeartFactory;
  }

  public void Initialize() {
    _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDiedSignal);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<EnemyDiedSignal>(OnEnemyDiedSignal);
  }

  public override void Setup(Level level) {
    _level = level as SurviveLevel;
    _level.InitializeLogging();

    _lastEnemySpawnTime = _level.EnemySpawnOffset;
    _lastAllySpawnTime = _level.AllySpawnOffset;

    if (_level.WithFallingObstacles) {
      _lastObstacleSpawnTime = Time.time + _level.FallingObstacleOffset;
    }

    DOTween.Sequence()
      .SetDelay(_level.PlatformActionDelay)
      .OnComplete(() => {
        _platformManager.SetAmountOfEnabledPlatforms(_level.AvailablePlatforms);
      });

    if (_level.Enemies.Length > 0) {
      SpawnEnemy();
    }
  }

  public override void Tick() {
    // Enemy spawning
    if (_lastSpawned != null && Time.time - _lastEnemySpawnTime > _lastSpawned.SpawnCooldownAfterSpawnedInSeconds && !_level.AllEnemiesAreSpawned) {
      SpawnEnemy();
    }
    
    // Obstacle spawning - only spawn if enemies are still spawning
    if (_level.WithFallingObstacles && Time.time - _lastObstacleSpawnTime > _level.FallingObstacleInterval && !_level.AllEnemiesAreSpawned) {
      _fallingObstacleManager.QueueSpawnsAtRandomX(_level.ConcurrentFallingObstacles);
      _lastObstacleSpawnTime = Time.time;
    }

    // Ally spawning
    if (Time.time - _lastAllySpawnTime > _level.AllySpawnInterval && !_level.AllEnemiesAreSpawned && _level.AlliesToSpawnLeft > 0) {
      SpawnAlly();
    } else if (Time.time - _lastAllySpawnTime > _level.AllySpawnInterval && _level.AllEnemiesAreSpawned && _level.AlliesToSpawnLeft > 0) {
      // If all enemies were spawned, spawn all allies at once

      // Spawn first one directly to prevent the level from being finished
      SpawnAlly();

      var allySpawnSequence = DOTween.Sequence();
      for (var i = 1; i < _level.AlliesToSpawnLeft; i++) { // we start at 1 since we've already spawned an enemy
        Debug.Log("Allies to spawn left: " + _level.AlliesToSpawnLeft);
        allySpawnSequence
          .Append(
            DOTween.Sequence()
            .SetDelay(i * .35f)
            .AppendCallback(() => {
              SpawnAlly();
              //var ally = _flyingHeartFactory.Create();
              //_level.LogAllySpawn(ally);
              //// Dont set `_lastAllySpawnTime`
            })
          );
      }

      _level.Allies = 0; // to prevent further spawns
    }

    // Level lifecycle handling
    if (_level.IsFinished) {
      SignalBus.Fire(new LevelFinishedSignal {
        Level = _level
      });
    }
  }

  public override void Cleanup() {
    _enemyFactory.CleanAllEntities();
  }

  private void SpawnAlly() {
    var ally = _flyingHeartFactory.Create();
    _level.LogAllySpawn(ally);
    _lastAllySpawnTime = Time.time;
  }

  private void SpawnEnemy() {
    var newSpawn = _level.GetRandomEnemyModel();
    var enemy = _enemyFactory.Create(newSpawn.Enemy);
    enemy.transform.position = _flyingEnemySpawnPoints.GetRandomSpawnPosition();

    //var createMethod = _enemyFactory.GetType().GetMethod("Create").MakeGenericMethod(newSpawn.Enemy.GetType());
    //createMethod.Invoke(this, new object[] { newSpawn.Enemy });

    _level.LogEnemySpawn(newSpawn.Enemy);
    _lastSpawned = newSpawn;
    _lastEnemySpawnTime = Time.time;
  }

  private void OnEnemyDiedSignal(EnemyDiedSignal signal) {
    _level.LogKill(signal.Enemy);
  }
}
