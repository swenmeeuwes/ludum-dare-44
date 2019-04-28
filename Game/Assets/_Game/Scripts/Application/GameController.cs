using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : IInitializable {
  private SignalBus _signalBus;
  private Player _player;
  private WeaponFactory _weaponFactory;
  private LevelController _levelController;

  private GameState _state;
  public GameState State {
    get => _state;
    set {
      OnGameStateChanged(_state, value);
      _state = value;
    }
  }

  [Inject]
  private void Construct(SignalBus signalBus, Player player, WeaponFactory weaponFactory, LevelController levelController) {
    _signalBus = signalBus;
    _player = player;
    _weaponFactory = weaponFactory;
    _levelController = levelController;
  }

  public void Initialize() {
    _player.Weapon = _weaponFactory.Create<Bow>();

    State = GameState.Intro;
  }

  private void PlayIntro() {
    // quick hack
    var introductionController = GameObject.FindObjectOfType<IntroductionController>();
    if (introductionController != null) {
      introductionController
        .PlayIntro()
        .Then(() => {
          _signalBus.Fire(new StartLevelSignal { LevelIndex = 0 });
          State = GameState.Playing;
        })
        .Catch(ex => Debug.LogError(ex));
    }
  }

  private void OnGameStateChanged(GameState oldSate, GameState newState) {
    switch (newState) {
      case GameState.Intro:
        PlayIntro();
        break;
      case GameState.Playing:
        
        break;
      case GameState.Paused:
        break;
      default:
        break;
    }
  }

  public enum GameState {
    Intro,
    Playing,
    Paused
  }
}
