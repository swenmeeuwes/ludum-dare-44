using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Bow : Weapon {
  private ProjectileFactory<Arrow> _projectileFactory;

  [Inject]
  private void Construct(ProjectileFactory<Arrow> projectileFactory) {
    _projectileFactory = projectileFactory;
  }

  public override bool Fire() {
    var canFire = base.Fire();
    if (!canFire) {
      return false;
    }

    var arrow = _projectileFactory.Create();
    arrow.transform.position = (Vector2)_renderer.transform.position;// + Direction * .15f;
    arrow.Rigidbody.velocity = Direction * ChargedFirePower;

    return true;
  }
}
