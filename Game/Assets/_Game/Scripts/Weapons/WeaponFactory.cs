using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class WeaponFactory {
  private DiContainer _container;
  private WeaponContext _weaponContext;

  [Inject]
  private void Construct(DiContainer container, WeaponContext weaponContext) {
    _container = container;
    _weaponContext = weaponContext;
  }

  public T Create<T>() where T : Weapon {
    var weapon = _weaponContext.Weapons.OfType<T>().FirstOrDefault();
    if (weapon == null) {
      throw new System.Exception($"No weapon of type {typeof(T).ToString()} exists in weapon context.");
    }

    return _container.InstantiatePrefabForComponent<T>(weapon);
  }
}
