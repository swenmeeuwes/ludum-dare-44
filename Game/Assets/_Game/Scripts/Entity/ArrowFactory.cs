using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Obsolete]
public class ArrowFactory : ProjectileFactory<Arrow>, IInitializable, ITickable {
  private Arrow _arrowPrefab;
  private float _arrowLifeSpan;

  private Transform _root;
  private List<ProjectileInfo<Arrow>> _projectiles = new List<ProjectileInfo<Arrow>>();

  [Inject]
  private void Construct(float arrowLifeSpan, Arrow arrowPrefab) {
    _arrowLifeSpan = arrowLifeSpan;
    _arrowPrefab = arrowPrefab;
  }

  public void Initialize() {
    _root = new GameObject("Arrows").transform;
  }

  public void Tick() {
    for (int i = _projectiles.Count - 1; i >= 0; i--) {
      var projectileInfo = _projectiles[i];
      if (Time.time - projectileInfo.CreationTime > _arrowLifeSpan) {
        _projectiles.Remove(projectileInfo);
        projectileInfo.Projectile.Disappear();
      }
    }
  }

  public Arrow Create() {
    var arrow = GameObject.Instantiate(_arrowPrefab, _root);
    _projectiles.Add(new ProjectileInfo<Arrow> {
      Projectile = arrow
    });
    
    return arrow;
  }
}