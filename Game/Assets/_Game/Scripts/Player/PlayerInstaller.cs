using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller {
  [SerializeField] private Player _player;
  [SerializeField] private PlayerMovement _movement;

  public override void InstallBindings() {
    Container
      .Bind<PlayerMovement>()
      .FromInstance(_movement);

    Container
      .Bind<Player>()
      .FromInstance(_player);
  }
}