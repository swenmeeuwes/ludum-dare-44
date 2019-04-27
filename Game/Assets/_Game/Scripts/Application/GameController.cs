using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : IInitializable {
  private SignalBus _signalBus;
  private Player _player;
  private WeaponFactory _weaponFactory;

  [Inject]
  private void Construct(SignalBus signalBus, Player player, WeaponFactory weaponFactory) {
    _signalBus = signalBus;
    _player = player;
    _weaponFactory = weaponFactory;
  }

  public void Initialize() {
    _player.Weapon = _weaponFactory.Create<Bow>();
  }
}
