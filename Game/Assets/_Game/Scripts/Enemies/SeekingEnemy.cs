using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SeekingEnemy : Enemy {
  [SerializeField] protected float MaxSpeed;

  public Transform Target { get; set; }

  private void Update() {
    if (Target != null) {
      SeekTarget();
    }
  }

  protected virtual void SeekTarget() {
    if (Target == null || IsDead) {
      return;
    }

    //var vectorToTarget = Target.position - transform.position;
    //transform.position += vectorToTarget.normalized * MovementSpeed * Time.deltaTime;

    // todo: Refactor to flying enemy?
    if (Rigidbody.velocity.magnitude < MaxSpeed) {
      var vectorToTarget = Target.position - transform.position;
      Rigidbody.AddForce(vectorToTarget.normalized * MovementSpeed);
    }
  }
}
