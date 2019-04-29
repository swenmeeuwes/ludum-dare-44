using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Player : MonoBehaviour {
  [SerializeField] private float _knockbackAmount = 2;
  [SerializeField] private int _initialMaxHealth = 10;
  [SerializeField] private float _secondsInvulnerableAfterHit = .65f;

  private SpriteRenderer _spriteRenderer;
  private Rigidbody2D _rigidbody;

  private PlayerMovement _movement;
  private SignalBus _signalBus;

  public PlayerMovement Movement { get => _movement; }

  private Weapon _weapon;
  public Weapon Weapon {
    get => _weapon;
    set {
      DetachCurrentWeapon();
      _weapon = value;
      AttachWeapon(_weapon);
    }
  }

  private bool _canMove;
  public bool CanMove {
    get => _canMove;
    set {
      _movement.CanMove = value;
      _canMove = value;
    }
  }

  private int _maxHealth;
  public int MaxHealth {
    get => _maxHealth;
    set {
      var oldMaxHealth = _maxHealth;
      _maxHealth = value;

      _signalBus.Fire(new PlayerMaxHealthChangedSignal {
        OldMaxHealth = oldMaxHealth,
        NewMaxHealth = value
      });
    }
  }

  private int _health;
  public int Health {
    get => _health;
    set {
      var oldHealth = _health;
      _health = value;

      _signalBus.Fire(new PlayerHealthChangedSignal {
        OldHealth = oldHealth,
        NewHealth = value
      });

      if (value < oldHealth) {
        ShowDamageTaken();
      }
    }
  }

  private float _timeSinceLastHit; // In seconds

  [Inject]
  private void Construct(SignalBus signalBus, PlayerMovement playerMovement) {
    _signalBus = signalBus;
    _movement = playerMovement;
  }

  private void Start() {
    _spriteRenderer = GetComponent<SpriteRenderer>();
    _rigidbody = GetComponent<Rigidbody2D>();

    MaxHealth = _initialMaxHealth;
    Health = _initialMaxHealth;
  }

  private void Update() {
    if (Weapon != null && CanMove) {
      var fireButton = InputAxes.Fire1;
      if (Input.GetButton(fireButton)) {
        Weapon.Charge();
      }
      else if (Input.GetButtonUp(fireButton)) {
        Weapon.Fire();
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D collider) {
    var enemy = collider.gameObject.GetComponent<Enemy>();
    if (enemy != null) {
      if (Time.time - _timeSinceLastHit > _secondsInvulnerableAfterHit) {
        _timeSinceLastHit = Time.time;
        Health -= enemy.Damage;

        var knockBackDirection = -(collider.transform.position - transform.position).normalized;
        _rigidbody.AddForce(knockBackDirection * _knockbackAmount, ForceMode2D.Impulse);

        Debug.LogFormat("Player was hit for {0} damage by an enemy!", enemy.Damage);
      }
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    var obstacle = collision.gameObject.GetComponent<Obstacle>();
    if (obstacle != null) {
      _timeSinceLastHit = Time.time;
      Health -= obstacle.Damage;

      var knockBackDirection = -(collision.transform.position - transform.position).normalized;
      _rigidbody.AddForce(knockBackDirection * _knockbackAmount, ForceMode2D.Impulse);

      Debug.LogFormat("Player was hit for {0} damage by an obstacle!", obstacle.Damage);
    }
  }

  private void ShowDamageTaken() {
    DOTween.Sequence()
      .Append(_spriteRenderer.DOFade(.2f, .1f))
      .Append(_spriteRenderer.DOFade(1, .1f))
      .SetLoops(3);
  }

  private void DetachCurrentWeapon() {
    if (Weapon == null) {
      return;
    }

    Destroy(Weapon);
    Weapon = null;
  }

  private void AttachWeapon(Weapon weapon) {
    weapon.transform.SetParent(transform, false);
  }
}
