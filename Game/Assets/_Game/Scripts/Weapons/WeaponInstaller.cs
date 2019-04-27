using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WeaponInstaller<TProjectile> : MonoInstaller where TProjectile : MonoBehaviour, IProjectile
{
  [SerializeField] protected TProjectile ProjectilePrefab;

  public override void InstallBindings() {
    Container
      .Bind<TProjectile>()
      .AsTransient();

    Container
      .BindInterfacesAndSelfTo<ProjectileFactory<TProjectile>>();
  }
}
