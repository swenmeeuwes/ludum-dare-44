using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SeekingEnemy : Enemy {
  public Transform Target { get; set; }

  private void Update() {
    if (Target != null) {
      SeekTarget();
    }
  }

  protected virtual void SeekTarget() {
    if (Target == null) {
      return;
    }

    var vectorToTarget = Target.position - transform.position;
    transform.position += vectorToTarget.normalized * MovementSpeed * Time.deltaTime;
  }
}
