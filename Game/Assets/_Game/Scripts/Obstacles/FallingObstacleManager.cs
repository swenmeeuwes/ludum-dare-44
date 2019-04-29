using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FallingObstacleManager : IInitializable, IDisposable, ITickable {
  private SignalBus _signalBus;
  private FallingObstaclesSpawnPoints _spawnPoints;
  private SpikeyBall _spikeyBallPrefab;
  private AlertArrowManager _alertArrowManager;

  private Transform _root;
  private readonly List<SpikeyBallInfo> _spawns = new List<SpikeyBallInfo>();
  private float _obstacleLifeSpanInSeconds = 10f;

  [Inject]
  private void Construct(SignalBus signalBus, FallingObstaclesSpawnPoints spawnPoints, SpikeyBall spikeyBallPrefab, AlertArrowManager alertArrowManager) {
    _signalBus = signalBus;
    _spawnPoints = spawnPoints;
    _spikeyBallPrefab = spikeyBallPrefab;
    _alertArrowManager = alertArrowManager;
  }

  public void Initialize() {
    _root = new GameObject("FallingObstacles").transform;
  }

  public void Dispose() {
    // Throws exceptions and this manager will be always active for now... todo: fix null check
    //if (_root.gameObject) {
    //  GameObject.Destroy(_root.gameObject);
    //}
  }

  public void Tick() {
    for (int i = _spawns.Count - 1; i >= 0; i--) {
      var spawn = _spawns[i];
      if (Time.time - spawn.CreationTime > _obstacleLifeSpanInSeconds) {
        GameObject.Destroy(spawn.SpikeyBall.gameObject);
        _spawns.RemoveAt(i);
      }
    }
  }

  public void QueueSpawnsAtRandomX(int amount = 1) {
    var spawnPositions = _spawnPoints.GetRandomSpawnPositions(amount);

    foreach (var spawnPosition in spawnPositions) {
      // Show alert before spawning
      _alertArrowManager.ShowAlertArrowAtWorldX(spawnPosition.x)
        .Then(() => {
          var spikeyBall = Create();
          spikeyBall.transform.position = spawnPosition;
        })
        .Catch(ex => Debug.LogError(ex));
    }
  }

  private SpikeyBall Create() {
    var spikeyBall = GameObject.Instantiate(_spikeyBallPrefab, _root);
    _spawns.Add(new SpikeyBallInfo {
      SpikeyBall = spikeyBall,
      CreationTime = Time.time
    });

    return spikeyBall;
  }

  private class SpikeyBallInfo {
    public SpikeyBall SpikeyBall;
    public float CreationTime;
  }
}
