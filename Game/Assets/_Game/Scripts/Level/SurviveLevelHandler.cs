﻿using DG.Tweening;
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

  private SurviveLevel _level;
  private float _lastEnemySpawnTime;
  private float _lastObstacleSpawnTime;

  private SurviveLevel.EnemyModel _lastSpawned;

  [Inject]
  private void Construct(SignalBus signalBus, EnemyFactory enemyFactory, FlyingEnemySpawnPoints flyingEnemySpawnPoints, FallingObstacleManager fallingObstacleManager, PlatformManager platformManager) {
    _signalBus = signalBus;
    _enemyFactory = enemyFactory;
    _flyingEnemySpawnPoints = flyingEnemySpawnPoints;
    _fallingObstacleManager = fallingObstacleManager;
    _platformManager = platformManager;
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

    if (_level.WithFallingObstacles) {
      _lastObstacleSpawnTime = Time.time + _level.FallingObstacleOffset;
    }

    DOTween.Sequence()
      .SetDelay(_level.PlatformActionDelay)
      .OnComplete(() => {
        _platformManager.SetAmountOfEnabledPlatforms(_level.AvailablePlatforms);
      });

    Spawn();
  }

  public override void Tick() {
    // Enemy spawning
    if (Time.time - _lastEnemySpawnTime > _lastSpawned.SpawnCooldownAfterSpawnedInSeconds && !_level.AllEnemiesAreSpawned) {
      Spawn();
    }
    
    // Obstacle spawning - only spawn if enemies are still spawning
    if (Time.time - _lastObstacleSpawnTime > _level.FallingObstacleInterval && !_level.AllEnemiesAreSpawned) {
      _fallingObstacleManager.QueueSpawnsAtRandomX(_level.ConcurrentFallingObstacles);
      _lastObstacleSpawnTime = Time.time;
    }

    if (_level.IsFinished) {
      SignalBus.Fire(new LevelFinishedSignal {
        Level = _level
      });
    }
  }

  public override void Cleanup() {
    _enemyFactory.CleanAllEntities();
  }

  private void Spawn() {
    var newSpawn = _level.GetRandomEnemyModel();
    var enemy = _enemyFactory.Create(newSpawn.Enemy);
    enemy.transform.position = _flyingEnemySpawnPoints.GetRandomSpawnPosition();

    //var createMethod = _enemyFactory.GetType().GetMethod("Create").MakeGenericMethod(newSpawn.Enemy.GetType());
    //createMethod.Invoke(this, new object[] { newSpawn.Enemy });

    _level.LogSpawn(newSpawn.Enemy);
    _lastSpawned = newSpawn;
    _lastEnemySpawnTime = Time.time;
  }

  private void OnEnemyDiedSignal(EnemyDiedSignal signal) {
    _level.LogKill(signal.Enemy);
  }
}
