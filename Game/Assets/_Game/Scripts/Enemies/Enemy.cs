using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator))]
public abstract class Enemy : MonoBehaviour {
  [SerializeField] protected float MovementSpeed;

  protected Collider2D Collider;
  protected Rigidbody2D Rigidbody;
  protected Animator Animator;

  private bool _isDead;
  public bool IsDead {
    get => _isDead;
    protected set {
      _isDead = value;
      if (value) {
        OnDead();
      }
    }
  }

  private SignalBus _signalBus;

  [Inject]
  private void Construct(SignalBus signalBus) {
    _signalBus = signalBus;
  }

  protected virtual void Awake() {
    Collider = GetComponent<Collider2D>();
    Rigidbody = GetComponent<Rigidbody2D>();
    Animator = GetComponent<Animator>();
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (IsDead) {
      return;
    }


    var projectile = collision.gameObject.GetComponent<IProjectile>();
    if (projectile != null) {
      IsDead = true;
    }
  }

  protected virtual void OnDead() {
    //Collider.enabled = false;
    Collider.isTrigger = false;
    Animator.SetBool("Dead", true);

    _signalBus.Fire(new EnemyDiedSignal {
      Enemy = this
    });
  }
}
