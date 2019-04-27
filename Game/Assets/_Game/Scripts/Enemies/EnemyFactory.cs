using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyFactory : IInitializable
{
  private EnemyContext _enemyContext;

  private Transform _root;

  private readonly List<Enemy> _enemies = new List<Enemy>();

  [Inject]
  private void Construct(EnemyContext enemyContext) {
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
}
