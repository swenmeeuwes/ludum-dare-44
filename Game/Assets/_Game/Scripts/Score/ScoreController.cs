using System;
using UnityEngine;
using Zenject;

public class ScoreController : IInitializable, IDisposable {
  private SignalBus _signalBus;

  private int _score;
  public int Score {
    get => _score;
    set {
      var oldScore = _score;
      _score = value;

      _signalBus.Fire(new ScoreChangedSignal {
        OldScore = oldScore,
        NewScore = value
      });
    }
  }

  public int HighScore {
    get => PlayerPrefs.GetInt(PlayerPrefKey.HighScore, 0);
  }

  [Inject]
  private void Construct(SignalBus signalBus) {
    _signalBus = signalBus;
  }

  public void Initialize() {
    _signalBus.Subscribe<AddScoreSignal>(OnScoreAdded);
    _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<AddScoreSignal>(OnScoreAdded);
    _signalBus.TryUnsubscribe<EnemyDiedSignal>(OnEnemyDied);
  }

  public bool SaveHighScore() {
    var currentHighScore = PlayerPrefs.GetInt(PlayerPrefKey.HighScore, 0);
    if (Score > currentHighScore) {
      PlayerPrefs.SetInt(PlayerPrefKey.HighScore, Score);
      return true;
    }

    return false;
  }

  private void OnScoreAdded(AddScoreSignal signal) {
    Score += signal.Amount;
  }

  private void OnEnemyDied(EnemyDiedSignal signal) {
    Score += signal.Enemy.RewardScore;
  }
}
