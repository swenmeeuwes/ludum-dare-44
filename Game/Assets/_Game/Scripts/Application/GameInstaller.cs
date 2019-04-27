using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller {
  [SerializeField] private WeaponContext _weaponContext;
  [SerializeField] private EnemyContext _enemyContext;

  private PrefabContext _prefabContext;

  [Inject]
  private void Construct(PrefabContext prefabContext) {
    _prefabContext = prefabContext;
  }

  public override void InstallBindings() {
    Container.Bind<WeaponContext>().FromInstance(_weaponContext).AsSingle();
    Container.BindInterfacesAndSelfTo<WeaponFactory>().AsSingle();
    Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

    Container.Bind<Arrow>().FromInstance(_prefabContext.Arrow).AsTransient();
    Container.BindInterfacesAndSelfTo<ProjectileFactory<Arrow>>().AsSingle();

    Container.Bind<EnemyContext>().FromInstance(_enemyContext).AsSingle();
    Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle();
  }
}