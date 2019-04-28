using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyFactory : IInitializable, IDisposable
{
  private DiContainer _container;
  private SignalBus _signalBus;
  private EnemyContext _enemyContext;

  private Transform _root;

  private readonly List<Enemy> _enemies = new List<Enemy>();

  [Inject]
  private void Construct(DiContainer container, SignalBus signalBus, EnemyContext enemyContext) {
    _container = container;
    _signalBus = signalBus;
    _enemyContext = enemyContext;
  }

  public void Initialize() {
    _root = new GameObject("Enemies").transform;

    _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<EnemyDiedSignal>(OnEnemyDied);
  }

  public T Create<T>() where T : Enemy {
    var enemy = _enemyContext.Enemies.OfType<T>().First();
    enemy.transform.SetParent(_root, false);
    _enemies.Add(enemy);

    return enemy;
  }

  public Enemy Create(Enemy enemyToBeCreated) {
    var enemyPrefab = _enemyContext.Enemies.First(e => e.GetType() == enemyToBeCreated.GetType());
    var enemy = _container.InstantiatePrefabForComponent<Enemy>(enemyPrefab);
    enemy.transform.SetParent(_root, false);
    _enemies.Add(enemy);

    return enemy;
  }

  public void CleanAllEntities() {
    foreach (var enemy in _enemies) {
      GameObject.Destroy(enemy.gameObject);
    }

    _enemies.Clear();
  }

  private void OnEnemyDied(EnemyDiedSignal signal) {
    _enemies.Remove(signal.Enemy);
  }
}
