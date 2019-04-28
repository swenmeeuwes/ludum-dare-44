using DG.Tweening;
using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Arrow : MonoBehaviour, IProjectile
{
  private Collider2D _collider;
  private Rigidbody2D _rigidbody;
  private SpriteRenderer _renderer;

  public Rigidbody2D Rigidbody { get => _rigidbody; }

  private void Awake() {
    _collider = GetComponent<Collider2D>();
    _rigidbody = GetComponent<Rigidbody2D>();
    _renderer = GetComponent<SpriteRenderer>();

    _collider.isTrigger = true;
  }

  private void Start() {
    _renderer.DOFade(0, 0);
    _renderer.DOFade(1, .25f);
    UpdateRotation();
  }

  private void Update() {
    if (_rigidbody.velocity.magnitude > 0) {
      UpdateRotation();
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    _collider.enabled = false;

    _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    _rigidbody.velocity = Vector2.zero;
    _rigidbody.Sleep();
    transform.DOShakeRotation(0.3f, new Vector3(0, 0, 35), 8);

    // Stick to object
    transform.SetParent(collision.transform, true);
  }

  private void UpdateRotation() {
    var angle = Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  }

  public IPromise Disappear() {
    var promise = new Promise();
    _renderer.DOFade(0f, 5f).OnComplete(() => {
      promise.Resolve();
    });

    return promise;
  }
}
