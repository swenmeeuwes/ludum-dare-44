using UnityEngine;

public class ProjectileInfo<T> {
  public float CreationTime { get; set; }
  public T Projectile { get; set; }

  public ProjectileInfo() {
    CreationTime = Time.time;
  }
}