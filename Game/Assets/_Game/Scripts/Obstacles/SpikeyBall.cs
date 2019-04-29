using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeyBall : Obstacle
{
  private void OnCollisionEnter2D(Collision2D collision) {
    StartCoroutine(QueueFallThrough());
  }

  private IEnumerator QueueFallThrough() {
    yield return new WaitForSeconds(.45f);

    Collider.enabled = false;
  }
}
