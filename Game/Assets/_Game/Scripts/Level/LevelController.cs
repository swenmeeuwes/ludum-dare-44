using System;
using UnityEngine;
using Zenject;

public class LevelController : IInitializable, IDisposable, ITickable {
  private SignalBus _signalBus;
  private LevelContext _levelContext;
  private LevelInterpreter _levelInterpreter;

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
  private void Construct(SignalBus signalBus, LevelContext levelContext, LevelInterpreter levelInterpreter) {
    _signalBus = signalBus;
    _levelContext = levelContext;
    _levelInterpreter = levelInterpreter;
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
  }

  private void TryRemoveSubscriptions() {
    _signalBus.TryUnsubscribe<StartLevelSignal>(OnStartLevelSignal);
    _signalBus.TryUnsubscribe<LevelFinishedSignal>(OnLevelFinishedSignal);
  }

  private void OnStartLevelSignal(StartLevelSignal signal) {
    StartLevel(signal.LevelIndex);
  }

  private void OnLevelFinishedSignal(LevelFinishedSignal signal) {
    IsRunning = false;

    Debug.Log("Level finsihed!");

    // Start next?
    // CurrentLevelIndex + 1
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
}
