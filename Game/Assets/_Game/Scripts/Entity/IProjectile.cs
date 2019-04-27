using RSG;
using UnityEngine;

public interface IProjectile {
  Rigidbody2D Rigidbody { get; }
  IPromise Disappear();
}
