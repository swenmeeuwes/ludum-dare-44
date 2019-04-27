using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller {
  [SerializeField] private PrefabContext _prefabContext;
  [SerializeField] private ScreenContext _screenContext;

  public override void InstallBindings() {
    InstallBindings(Container);
    InstallSignals(Container);
  }

  private void InstallBindings(DiContainer container) {
    container.Bind<PrefabContext>().FromInstance(_prefabContext).AsSingle();
    container.Bind<ScreenRoot>().FromComponentInNewPrefab(_prefabContext.screenRoot).AsSingle();
    container.BindInterfacesAndSelfTo<ScreenFactory>().AsSingle().WithArguments(_screenContext.config);
    container.BindInterfacesAndSelfTo<ScreenNavigator>().AsSingle();
    container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
    container.BindInterfacesAndSelfTo<ApplicationController>().AsSingle().NonLazy();
  }

  private void InstallSignals(DiContainer container) {
    SignalBusInstaller.Install(container);

    container.DeclareSignal<OpenScreenRequestSignal>();
    container.DeclareSignal<CloseScreenRequestSignal>();
    container.DeclareSignal<GotoScreenRequestSignal>();
  }
}