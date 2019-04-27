using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ApplicationController : IInitializable {
  private SignalBus _signalBus;
  private ScreenNavigator _screenNavigator;

  public ApplicationController(SignalBus signalBus, ScreenNavigator screenNavigator) {
    _signalBus = signalBus;
    _screenNavigator = screenNavigator;
  }

  public void Initialize() {
    DOTween.Init();

    //_signalBus.Fire(new GotoScreenRequestSignal {
    //  ScreenType = typeof(LoginScreen)
    //});
  }
}
