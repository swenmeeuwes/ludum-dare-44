using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScreenNavigator : IInitializable, IDisposable {
  private readonly List<ScreenController> _navigationStack = new List<ScreenController>();

  private SignalBus _signalBus;
  private ScreenFactory _screenFactory;
  public ScreenNavigator(SignalBus signalBus, ScreenFactory screenFactory) {
    _signalBus = signalBus;
    _screenFactory = screenFactory;
  }

  public void Initialize() {
    RegisterListeners();
  }

  public void Dispose() {
    DeregisterListeners();
  }

  private void RegisterListeners() {
    _signalBus.Subscribe<OpenScreenRequestSignal>(OnOpenScreenRequestSignal);
    _signalBus.Subscribe<CloseScreenRequestSignal>(OnCloseScreenRequestSignal);
    _signalBus.Subscribe<GotoScreenRequestSignal>(OnGotoScreenRequestSignal);
  }

  private void DeregisterListeners() {
    _signalBus.TryUnsubscribe<OpenScreenRequestSignal>(OnOpenScreenRequestSignal);
    _signalBus.TryUnsubscribe<CloseScreenRequestSignal>(OnCloseScreenRequestSignal);
    _signalBus.TryUnsubscribe<GotoScreenRequestSignal>(OnGotoScreenRequestSignal);
  }

  private void OnOpenScreenRequestSignal(OpenScreenRequestSignal signal) {
    var openMethod = GetType().GetMethod("OpenScreen").MakeGenericMethod(signal.ScreenType);
    openMethod.Invoke(this, new object[] { signal.Animation });
  }

  private void OnCloseScreenRequestSignal(CloseScreenRequestSignal signal) {
    CloseScreen(signal.Screen, signal.Animation);
  }

  private void OnGotoScreenRequestSignal(GotoScreenRequestSignal signal) {
    var openMethod = GetType().GetMethod("GotoScreen").MakeGenericMethod(signal.ScreenType);
    openMethod.Invoke(this, new object[] { signal.OpeningAnimation, signal.ClosingAnimation });
  }

  public ScreenController GotoScreen<T>(IUIAnimation openingScreenAnimation = null, IUIAnimation closingScreenAnimation = null) where T : ScreenController {
    CloseAll(closingScreenAnimation);
    return OpenScreen<T>(openingScreenAnimation);
  }

  public ScreenController OpenScreen<T>(IUIAnimation animation = null) where T : ScreenController {
    var screen = _screenFactory.Create<T>();
    _navigationStack.Add(screen);

    if (animation != null) {
      animation.AnimateOn(screen.GetComponent<RectTransform>());
    }

    return screen;
  }

  private void CloseScreen(ScreenController screen, IUIAnimation animation = null) {
    _navigationStack.Remove(screen);

    if (animation == null) {
      GameObject.Destroy(screen.gameObject);
      return;
    }

    animation.AnimateOn(screen.GetComponent<RectTransform>())
        .Then(() => GameObject.Destroy(screen.gameObject));
  }

  private void CloseAll(IUIAnimation animation = null) {
    foreach (var screen in _navigationStack) {
      if (animation == null) {
        GameObject.Destroy(screen.gameObject);
        continue;
      }

      animation.AnimateOn(screen.GetComponent<RectTransform>())
          .Then(() => GameObject.Destroy(screen.gameObject));
    }

    _navigationStack.Clear();
  }
}