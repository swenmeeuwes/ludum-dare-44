using System;
using UnityEngine;
using Zenject;

public class LevelController : IInitializable, IDisposable, ITickable {
  private SignalBus _signalBus;
  private LevelContext _levelContext;
  private LevelInterpreter _levelInterpreter;
  private CurtainController _curtainController;
  private Player _player;
  private Shop _shop;

  public bool IsRunning { get; private set; }

  public int CurrentLevelIndex { get; private set; }

  private Level _currentLevel;
  public Level CurrentLevel {
    get => _currentLevel;
    private set {
      if (IsRunning) {
        Debug.LogError("Cannot change level while still playing it!");
        return;
      }

      _currentLevel = value;
    }
  }

  [Inject]
  private void Construct(SignalBus signalBus, LevelContext levelContext, LevelInterpreter levelInterpreter, CurtainController curtainController, Player player, Shop shop) {
    _signalBus = signalBus;
    _levelContext = levelContext;
    _levelInterpreter = levelInterpreter;
    _curtainController = curtainController;
    _player = player;
    _shop = shop;
  }

  public void Initialize() {
    AddSubscriptions();
  }

  public void Dispose() {
    TryRemoveSubscriptions();
  }

  public void Tick() {
    if (CurrentLevel == null || !IsRunning) {
      return;
    }

    _levelInterpreter.Interpret(CurrentLevel).Tick();
  }

  private void AddSubscriptions() {
    _signalBus.Subscribe<StartLevelSignal>(OnStartLevelSignal);
    _signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinishedSignal);
    _signalBus.Subscribe<ShopClosedSignal>(OnShopClosedSignal);
  }

  private void TryRemoveSubscriptions() {
    _signalBus.TryUnsubscribe<StartLevelSignal>(OnStartLevelSignal);
    _signalBus.TryUnsubscribe<LevelFinishedSignal>(OnLevelFinishedSignal);
    _signalBus.TryUnsubscribe<ShopClosedSignal>(OnShopClosedSignal);
  }

  private void OnStartLevelSignal(StartLevelSignal signal) {
    StartLevel(signal.LevelIndex);
  }

  private void OnLevelFinishedSignal(LevelFinishedSignal signal) {
    IsRunning = false;

    _player.CanMove = false;

    if (CurrentLevelIndex + 1 >= _levelContext.Levels.Length) {
      // The player finished the game!
      _signalBus.Fire(new AllLevelsCompletedSignal { });
      return;
    }

    //var showShop = UnityEngine.Random.Range(0, 1) == 1;
    var showShop = (CurrentLevelIndex + 1) % 2 == 0; // Show shop on 3th, 5th, 7th... level
    if (showShop) {
      _curtainController
        .ShowShop("Shop Time!")
        .Then(() => {
          _shop.ShowRandomItems();
        })
        .Catch(ex => Debug.LogError(ex));
    }
    else {
      PlayNextLevel();
    }
  }

  private void OnShopClosedSignal(ShopClosedSignal signal) {
    PlayNextLevel();
  }

  private void StartLevel(int levelIndex) {
    if (levelIndex >= _levelContext.Levels.Length) {
      Debug.LogError("Level index out of bounds");
      return;
    }

    CurrentLevelIndex = levelIndex;
    CurrentLevel = GameObject.Instantiate(_levelContext.Levels[levelIndex]);
    var levelHandler = _levelInterpreter.Interpret(CurrentLevel);
    levelHandler.Setup(CurrentLevel);

    IsRunning = true;
  }

  private void PlayNextLevel() {
    // Continue playing the next level
    _curtainController
      .ShowEnteringRoundText(string.Format("Round {0}", CurrentLevelIndex + 2)) // +2 because CurrentLevelIndex is 0 index based + we want the next
      .Then(() => {
        _signalBus.Fire(new StartLevelSignal { LevelIndex = CurrentLevelIndex + 1 });

        _player.CanMove = true;
      })
      .Catch(ex => Debug.LogError(ex));
  }
}
