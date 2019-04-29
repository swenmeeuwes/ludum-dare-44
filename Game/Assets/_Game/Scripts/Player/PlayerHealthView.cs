using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

public class PlayerHealthView : HealthView, IInitializable, IDisposable {
  [SerializeField] private int _shakeAtHealthLowerThan = 5;

  private SignalBus _signalBus;

  private Tweener _heartsShakeTweener;

  [Inject]
  private void Construct(SignalBus signalBus, HeartView heartView) {
    _signalBus = signalBus;
  }

  public void Initialize() {
    _signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
    _signalBus.Subscribe<PlayerMaxHealthChangedSignal>(OnPlayerMaxHealthChangedSignal);
  }

  public void Dispose() {
    _signalBus.TryUnsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
    _signalBus.TryUnsubscribe<PlayerMaxHealthChangedSignal>(OnPlayerMaxHealthChangedSignal);
  }

  private void OnPlayerHealthChangedSignal(PlayerHealthChangedSignal signal) {
    if (signal.OldHealth > signal.NewHealth) {
      ShowDamageTaken();
    }

    if (_heartsShakeTweener != null && signal.NewHealth >= _shakeAtHealthLowerThan) {
      //_heartsShakeTweener.Complete();
      //_heartsShakeTweener = null;
    } else if (_heartsShakeTweener == null && signal.NewHealth < _shakeAtHealthLowerThan) {
      //var originalLocalScale = HeartViews[0].transform.localScale;
      //_heartsShakeTweener = HeartViews[0].transform.DOShakeScale(.5f, strength: .1f).SetLoops(-1);
    }

    ShowHealth(signal.NewHealth);
  }

  private void OnPlayerMaxHealthChangedSignal(PlayerMaxHealthChangedSignal signal) {
    EnsureEnoughHeartsFor(signal.NewMaxHealth);
  }
}
