using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : IInitializable {
  private SignalBus _signalBus;
  private Player _player;
  private WeaponFactory _weaponFactory;

  private GameState _state;
  public GameState State {
    get => _state;
    set {
      OnGameStateChanged(_state, value);
      _state = value;
    }
  }

  [Inject]
  private void Construct(SignalBus signalBus, Player player, WeaponFactory weaponFactory) {
    _signalBus = signalBus;
    _player = player;
    _weaponFactory = weaponFactory;
  }

  public void Initialize() {
    _player.Weapon = _weaponFactory.Create<Bow>();

    PlayIntro();
  }

  private void PlayIntro() {

  }

  private void OnGameStateChanged(GameState oldSate, GameState newState) {

  }

  public enum GameState {
    Intro,
    Playing,
    Paused
  }
}
