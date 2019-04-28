using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyFactory : IInitializable
{
  private DiContainer _container;
  private EnemyContext _enemyContext;

  private Transform _root;

  private readonly List<Enemy> _enemies = new List<Enemy>();

  [Inject]
  private void Construct(DiContainer container, EnemyContext enemyContext) {
    _container = container;
    _enemyContext = enemyContext;
  }

  public void Initialize() {
    _root = new GameObject("Enemies").transform;
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
}
