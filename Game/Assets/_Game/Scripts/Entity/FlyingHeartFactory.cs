using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class FlyingHeartFactory : IInitializable, IDisposable {
  private DiContainer _container;
  private SignalBus _signalBus;
  private FlyingHeart _flyingHeartPrefab;
  private FlyingHeartSpawnPoints _spawnPoints;

  private Transform _root;

  private readonly List<FlyingHeart> _hearts = new List<FlyingHeart>();

  [Inject]
  private void Construct(DiContainer container, SignalBus signalBus, FlyingHeart flyingHeart, FlyingHeartSpawnPoints flyingHeartSpawnPoints) {
    _container = container;
    _signalBus = signalBus;
    _flyingHeartPrefab = flyingHeart;
    _spawnPoints = flyingHeartSpawnPoints;
  }

  public void Initialize() {
    _root = new GameObject("Allies").transform;
  }

  public void Dispose() {

  }

  public FlyingHeart Create() {
    var ally = _container.InstantiatePrefabForComponent<FlyingHeart>(_flyingHeartPrefab);
    ally.transform.SetParent(_root, false);
    ally.transform.position = _spawnPoints.GetRandomSpawnPosition();

    _hearts.Add(ally);

    return ally;
  }

  public void CleanAllEntities() {
    foreach (var enemy in _hearts) {
      if (enemy != null) {
        GameObject.Destroy(enemy.gameObject);
      }
    }

    _hearts.Clear();
  }
}
