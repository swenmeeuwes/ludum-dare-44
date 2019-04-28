using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class LevelHandler {
  public abstract void Setup(Level level);
  public abstract void Tick();

  protected SignalBus SignalBus;

  [Inject]
  private void Construct(SignalBus signalBus) {
    SignalBus = signalBus;
  }
}
