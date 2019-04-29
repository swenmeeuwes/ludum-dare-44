using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FlyingEnemy : SeekingEnemy {
  [Inject] private Player _player;

  private void Start() {
    Target = _player.transform;

    Rigidbody.gravityScale = 0;
  }

  protected override void OnDead() {
    base.OnDead();

    Rigidbody.gravityScale = 1; // Fall out of the air
  }
}
