using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectileFactory<T> : IInitializable, ITickable where T : MonoBehaviour, IProjectile {
  private T _projectilePrefab;
  private float _projectileLifeSpan = 30f;

  private Transform _root;
  private List<ProjectileInfo<T>> _projectiles = new List<ProjectileInfo<T>>();

  [Inject]
  private void Construct(T projectilePrefab) {
    _projectilePrefab = projectilePrefab;
  }

  public void Initialize() {
    _root = new GameObject("Projectiles" + typeof(T).ToString()).transform;
  }

  public void Tick() {
    for (int i = _projectiles.Count - 1; i >= 0; i--) {
      var projectileInfo = _projectiles[i];
      if (Time.time - projectileInfo.CreationTime > _projectileLifeSpan) {
        _projectiles.Remove(projectileInfo);
        projectileInfo.Projectile.Disappear();
      }
    }
  }

  public T Create() {
    var projectile = GameObject.Instantiate(_projectilePrefab, _root);
    _projectiles.Add(new ProjectileInfo<T> {
      Projectile = projectile
    });

    return projectile;
  }
}