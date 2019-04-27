using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionCheck : MonoBehaviour {
  public bool IsColliding { get; private set; }

  private void OnTriggerEnter2D(Collider2D other) {
    IsColliding = true;
  }

  private void OnTriggerStay2D(Collider2D other) {
    IsColliding = true;
  }

  private void OnTriggerExit2D(Collider2D other) {
    IsColliding = false;
  }
}