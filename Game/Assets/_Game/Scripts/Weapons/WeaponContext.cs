using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Context", menuName = "Context /Weapon Context")]
public class WeaponContext : ScriptableObject {
  public Weapon[] Weapons;
}