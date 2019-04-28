using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SurviveLevelHandler : LevelHandler, IInitializable, IDisposable {
  private SignalBus _signalBus;
  private EnemyFactory _enemyFactory;
  private FlyingEnemySpawnPoints _flyingEnemySpawnPoints;

  private SurviveLevel _level;
  private float _lastSpawnTime;

  private SurviveLevel.EnemyModel _lastSpawned;

  [Inject]
  private void Construct(SignalBus signalBus, EnemyFactory enemyFactory, FlyingEnemySpawnPoints flyingEnemySpawnPoints) {
    _signalBus = signalBus;
    _enemyFactory = enemyFactory;
    _flyingEnemySpawnPoints = flyingEnemySpawnPoints;
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

    _lastSpawnTime = 0;

    Spawn();
  }

  public override void Tick() {
    if (Time.time -_lastSpawnTime > _lastSpawned.SpawnCooldownAfterSpawnedInSeconds && !_level.AllEnemiesAreSpawned) {
      Spawn();
    }

    if (_level.IsFinished) {
      SignalBus.Fire(new LevelFinishedSignal {
        Level = _level
      });
    }
  }

  private void Spawn() {
    var newSpawn = _level.GetRandomEnemyModel();
    var enemy = _enemyFactory.Create(newSpawn.Enemy);
    enemy.transform.position = _flyingEnemySpawnPoints.GetRandomSpawnPosition();

    //var createMethod = _enemyFactory.GetType().GetMethod("Create").MakeGenericMethod(newSpawn.Enemy.GetType());
    //createMethod.Invoke(this, new object[] { newSpawn.Enemy });

    _level.LogSpawn(newSpawn.Enemy);
    _lastSpawned = newSpawn;
    _lastSpawnTime = Time.time;
  }

  private void OnEnemyDiedSignal(EnemyDiedSignal signal) {
    _level.LogKill(signal.Enemy);
  }
}
