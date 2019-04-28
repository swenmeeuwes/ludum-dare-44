using System;
using UnityEngine;
using Zenject;

public class PlayerHealthView : HealthView, IInitializable, IDisposable {
  private SignalBus _signalBus;

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

    ShowHealth(signal.NewHealth);
  }

  private void OnPlayerMaxHealthChangedSignal(PlayerMaxHealthChangedSignal signal) {
    EnsureEnoughHeartsFor(signal.NewMaxHealth);
  }
}
