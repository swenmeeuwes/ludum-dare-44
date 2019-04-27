using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ScreenFactory : IDisposable {
  private IEnumerable<ScreenController> _screens;
  private ScreenRoot _screenRoot;
  private DiContainer _container;

  public ScreenFactory(IEnumerable<ScreenController> screens, ScreenRoot screenRoot, DiContainer container) {
    _screens = screens;
    _screenRoot = screenRoot;
    _container = container;
  }

  public void Dispose() {
    GameObject.Destroy(_screenRoot);
    _screenRoot = null;
  }

  public ScreenController Create<T>() where T : ScreenController {
    var screenPrefab = _screens.OfType<T>().First();
    var screenController = _container.InstantiatePrefabForComponent<T>(screenPrefab, _screenRoot.Canvas.transform);

    return screenController;
  }
}
