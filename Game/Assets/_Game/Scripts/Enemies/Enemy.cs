﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator))]
public abstract class Enemy : MonoBehaviour {
  [SerializeField] protected float MovementSpeed;
  [SerializeField] private int _initialDamage;
  [SerializeField] private int _rewardScore = 10;

  protected Collider2D Collider;
  protected Rigidbody2D Rigidbody;
  protected Animator Animator;
  protected SpriteRenderer SpriteRenderer;

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

  public int RewardScore { get => _rewardScore; }

  public int Damage { get => _initialDamage; }

  private SignalBus _signalBus;

  [Inject]
  private void Construct(SignalBus signalBus) {
    _signalBus = signalBus;
  }

  protected virtual void Awake() {
    Collider = GetComponent<Collider2D>();
    Rigidbody = GetComponent<Rigidbody2D>();
    Animator = GetComponent<Animator>();
    SpriteRenderer = GetComponent<SpriteRenderer>();
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

    DOTween.Sequence()
      .PrependInterval(5f)
      .Append(SpriteRenderer.DOFade(0, .45f))
      .OnComplete(() => Destroy(gameObject));
  }
}
