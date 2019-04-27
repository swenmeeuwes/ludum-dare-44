using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour {
  private PlayerMovement _movement;

  private Weapon _weapon;
  public Weapon Weapon {
    get => _weapon;
    set {
      DetachCurrentWeapon();
      _weapon = value;
      AttachWeapon(_weapon);
    }
  }

  [Inject]
  private void Construct(PlayerMovement playerMovement) {
    _movement = playerMovement;
  }

  private void Update() {
    if (Weapon != null) {
      var fireButton = InputAxes.Fire1;
      if (Input.GetButton(fireButton)) {
        Weapon.Charge();
      }
      else if (Input.GetButtonUp(fireButton)) {
        Weapon.Fire();
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
