using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeyBall : Obstacle {
  [SerializeField] private float _visualSpinTorque = 2f;

  public bool IsSpinningVisually { get; private set; }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (GameCamera.Instance != null) {
      GameCamera.Instance.Shake();
    }

    IsSpinningVisually = false;

    StartCoroutine(QueueFallThrough());
  }

  private void Start() {
    IsSpinningVisually = true;

    _visualSpinTorque += Random.Range(-100f, 100f);
  }

  private void Update() {
    if (IsSpinningVisually) {
      transform.Rotate(new Vector3(0, 0, 1), _visualSpinTorque * Time.deltaTime);
    }
  }

  private IEnumerator QueueFallThrough() {
    yield return new WaitForSeconds(.45f);

    Collider.enabled = false;
  }
}
