using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour {
  [SerializeField] private int _initialMaxHealth = 10;
  [SerializeField] private float _secondsInvulnerableAfterHit = .65f;

  private PlayerMovement _movement;
  private SignalBus _signalBus;

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
    }
  }

  private float _timeSinceLastHit; // In seconds

  [Inject]
  private void Construct(SignalBus signalBus, PlayerMovement playerMovement) {
    _signalBus = signalBus;
    _movement = playerMovement;
  }

  private void Start() {
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

        Debug.LogFormat("Player was hit for {0} damage!", enemy.Damage);
      }
    }
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
