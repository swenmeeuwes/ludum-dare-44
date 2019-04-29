using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreView : MonoBehaviour, IInitializable, IDisposable {
  [SerializeField] private TMP_Text _scoreText;
  [SerializeField] private float _incrementDuration; // In seconds
  [SerializeField] private int _numberAmount;

  private SignalBus _signalBus;

  private int _showingScore = 0;
  private int _targetScore = 0;
  private float _incrementStep;

  private string _formatString;

  [Inject]
  private void Construct(SignalBus signalBus) {
    _signalBus = signalBus;
  }

  public void Initialize() {
    _incrementStep = 1 / _incrementDuration;
    
    // todo: use string builder
    var scoreFormatString = "{0:";
    for (var i = 0; i < _numberAmount; i++) {
      scoreFormatString += "0";
    }
    scoreFormatString += "}";
    _formatString = scoreFormatString;

    _signalBus.Subscribe<ScoreChangedSignal>(OnScoreChanged);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<ScoreChangedSignal>(OnScoreChanged);
  }

  public void Update() {
    MoveScoreCounterTowardsScore(_targetScore);
  }

  private void OnScoreChanged(ScoreChangedSignal signal) {
    _targetScore = signal.NewScore;
  }

  private void MoveScoreCounterTowardsScore(int targetScore) {
    var scoreDelta = _targetScore - _showingScore;
    var scoreIncrement = Mathf.Ceil(scoreDelta * _incrementStep * Time.deltaTime);

    var newScore = Mathf.FloorToInt(_showingScore + scoreIncrement);
    var newScoreText = string.Format(_formatString, newScore);
    _scoreText.text = newScoreText;

    _showingScore = newScore;
  }
}
