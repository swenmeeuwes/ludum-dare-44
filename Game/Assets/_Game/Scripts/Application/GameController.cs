﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, IDisposable {
  private SignalBus _signalBus;
  private Player _player;
  private WeaponFactory _weaponFactory;
  private LevelController _levelController;
  private CurtainController _curtainController;

  private GameState _state;
  public GameState State {
    get => _state;
    set {
      OnGameStateChanged(_state, value);
      _state = value;
    }
  }

  [Inject]
  private void Construct(SignalBus signalBus, Player player, WeaponFactory weaponFactory, LevelController levelController, CurtainController curtainController) {
    _signalBus = signalBus;
    _player = player;
    _weaponFactory = weaponFactory;
    _levelController = levelController;
    _curtainController = curtainController;
  }

  public void Initialize() {
    AddSubscriptions();

    _player.CanMove = false;
    _player.Weapon = _weaponFactory.Create<Bow>();

    State = GameState.Intro;
  }

  public void Dispose() {
    TryRemoveSubscription();
  }

  private void AddSubscriptions() {
    _signalBus.Subscribe<ShopItemBoughtSignal>(OnShopItemBought);
    _signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
  }

  private void TryRemoveSubscription() {
    _signalBus.TryUnsubscribe<ShopItemBoughtSignal>(OnShopItemBought);
    _signalBus.TryUnsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
  }

  private void OnShopItemBought(ShopItemBoughtSignal signal) {
    _player.Health -= signal.Cost;
  }

  private void OnPlayerHealthChangedSignal(PlayerHealthChangedSignal signal) {
    if (signal.NewHealth <= 0) {
      State = GameState.GameOver;
    }
  }

  private void PlayIntro() {
    var introductionController = GameObject.FindObjectOfType<IntroductionController>(); // quick hack
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

  private void OnGameStateChanged(GameState oldState, GameState newState) {
    switch (newState) {
      case GameState.Intro:
        PlayIntro();
        break;
      case GameState.Playing:
        _player.CanMove = true;
        break;
      case GameState.Paused:
        _player.CanMove = false;
        break;
      case GameState.GameOver:
        _player.CanMove = false;

        if (oldState != GameState.GameOver) {
          _curtainController.ShowGameOver("Game Over");
        }
        break;
      default:
        break;
    }
  }

  public enum GameState {
    Intro,
    Playing,
    Paused,
    GameOver
  }
}
