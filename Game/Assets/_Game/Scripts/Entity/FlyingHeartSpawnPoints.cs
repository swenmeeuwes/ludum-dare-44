using UnityEngine;

public class FlyingHeartSpawnPoints : SpawnPoints {
  public override Vector2 GetRandomSpawnPosition() {
    var randomPositon = base.GetRandomSpawnPosition();
    randomPositon.y += Random.Range(-.5f, .5f);

    return randomPositon;
  }
}
