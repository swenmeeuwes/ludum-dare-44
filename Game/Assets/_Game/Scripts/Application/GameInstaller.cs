using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller {
  [SerializeField] private WeaponContext _weaponContext;
  [SerializeField] private EnemyContext _enemyContext;
  [SerializeField] private LevelContext _levelContext;

  private PrefabContext _prefabContext;

  [Inject]
  private void Construct(PrefabContext prefabContext) {
    _prefabContext = prefabContext;
  }

  public override void InstallBindings() {
    Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

    // Weapons
    Container.Bind<WeaponContext>().FromInstance(_weaponContext).AsSingle();
    Container.BindInterfacesAndSelfTo<WeaponFactory>().AsSingle();

    // Projectiles
    Container.Bind<Arrow>().FromInstance(_prefabContext.Arrow).AsTransient();
    Container.BindInterfacesAndSelfTo<ProjectileFactory<Arrow>>().AsSingle();

    // Enemies
    Container.Bind<EnemyContext>().FromInstance(_enemyContext).AsSingle();
    Container.BindInterfacesAndSelfTo<EnemyFactory>().AsSingle();

    Container.Bind<FlyingEnemySpawnPoints>().FromComponentInNewPrefab(_prefabContext.FlyingEnemySpawnPoints).AsSingle();

    Container.DeclareSignal<EnemyDiedSignal>().OptionalSubscriber();

    // Levels
    Container.Bind<LevelContext>().FromInstance(_levelContext).AsSingle();
    Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().NonLazy();

    Container.BindInterfacesAndSelfTo<LevelInterpreter>().AsSingle();
    Container.BindInterfacesAndSelfTo<SurviveLevelHandler>().AsSingle();

    Container.DeclareSignal<StartLevelSignal>().OptionalSubscriber();
    Container.DeclareSignal<LevelFinishedSignal>().OptionalSubscriber();

    // Health view
    Container.Bind<HeartView>().FromInstance(_prefabContext.HeartView).AsTransient();

    Container.DeclareSignal<PlayerMaxHealthChangedSignal>().OptionalSubscriber();
    Container.DeclareSignal<PlayerHealthChangedSignal>().OptionalSubscriber();

    // Execution Order
    Container.BindExecutionOrder<LevelController>(5); // Before GameController
    Container.BindExecutionOrder<GameController>(10);
  }
}