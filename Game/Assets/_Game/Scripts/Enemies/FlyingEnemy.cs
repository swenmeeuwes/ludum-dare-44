using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : SeekingEnemy {
  private void Start() {
    Target = GameObject.Find("Player").transform; // TEMP TEST

    Rigidbody.gravityScale = 0;
  }

  protected override void OnDead() {
    base.OnDead();

    Rigidbody.gravityScale = 1; // Fall out of the air
  }
}
