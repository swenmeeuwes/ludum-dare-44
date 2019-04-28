using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HealthView : MonoBehaviour, IInitializable, IDisposable
{
  private HorizontalLayoutGroup _layoutGroup;

  private SignalBus _signalBus;
  private HeartView _heartViewPrefab;

  private readonly List<HeartView> _heartViews = new List<HeartView>();

  [Inject]
  private void Construct(SignalBus signalBus, HeartView heartView) {
    _signalBus = signalBus;
    _heartViewPrefab = heartView;
  }

  private void Awake() {
    _layoutGroup = GetComponent<HorizontalLayoutGroup>();
  }

  public void Initialize() {
    _signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
    _signalBus.Subscribe<PlayerMaxHealthChangedSignal>(OnPlayerMaxHealthChangedSignal);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
    _signalBus.TryUnsubscribe<PlayerMaxHealthChangedSignal>(OnPlayerMaxHealthChangedSignal);
  }

  public void ShowHealth(int amount) {
    var healthPerHeart = _heartViewPrefab.HealthPointsAbleToShow;
    var healthToShowLeft = amount;
    foreach (var heartView in _heartViews) {
      if (healthToShowLeft <= 0) {
        heartView.State = 0;
        continue;
      }

      heartView.State = Mathf.Clamp(healthToShowLeft, 0, healthPerHeart);
      healthToShowLeft -= healthPerHeart;

      heartView.transform.DOPunchScale(Vector3.one, .05f);
    }
  }

  public void ShowAmountOfHearts(int maxHearts) {
    var heartsToAdd = maxHearts - _heartViews.Count;
    if (heartsToAdd > 0) {
      AddHearts(heartsToAdd);
    } else if (heartsToAdd < 0) {
      RemoveHearts(heartsToAdd);
    }
  }

  private void AddHearts(int amount) {
    for (int i = 0; i < amount; i++) {
      var heartView = GameObject.Instantiate(_heartViewPrefab);
      heartView.transform.SetParent(_layoutGroup.transform, false);

      DOTween.Sequence()
        .PrependInterval(i * .15f)
        .Append(heartView.transform.DOPunchScale(Vector3.one, .15f));

      _heartViews.Add(heartView);
    }
  }

  private void RemoveHearts(int amount) {
    throw new NotImplementedException();
  }

  private void OnPlayerHealthChangedSignal(PlayerHealthChangedSignal signal) {
    ShowHealth(signal.NewHealth);
  }

  private void OnPlayerMaxHealthChangedSignal(PlayerMaxHealthChangedSignal signal) {
    ShowAmountOfHearts(Mathf.CeilToInt((float)signal.NewMaxHealth / _heartViewPrefab.HealthPointsAbleToShow));
  }
}
