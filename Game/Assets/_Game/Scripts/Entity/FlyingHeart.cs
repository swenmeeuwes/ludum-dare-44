using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))]
public class FlyingHeart : MonoBehaviour {
  [SerializeField] private ParticleSystem _dieParticleSytem;
  [SerializeField] private float _movementSpeed = 2f;
  [SerializeField] private int _healAmount = 4;

  [Inject] private Player _player;

  private SpriteRenderer _spriteRenderer;

  private Vector2 _moveDirection;
  private float _diePositionX;

  public bool Alive { get; private set; }
  public int HealAmount { get => _healAmount; }

  private void Start() {
    _spriteRenderer = GetComponent<SpriteRenderer>();

    Alive = true;

    var vectorToPlayer = _player.transform.position - transform.position;
    _moveDirection = new Vector2(vectorToPlayer.normalized.x, 0);

    var cameraWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
    if (_moveDirection.x > 0) {
      _diePositionX = cameraWidth + .5f;
    } else {
      _diePositionX = -cameraWidth - .5f;
    }

    // Quick hacky cleanup after X seconds
    //DOTween.Sequence()
    //  .SetDelay(30f)
    //  .Append(_spriteRenderer.DOFade(0, .45f))
    //  .OnComplete(() => Destroy(gameObject));
  }

  private void Update() {
    if (_moveDirection.x > 0) {
      if (transform.position.x > _diePositionX) {
        Destroy(gameObject);
      }
    } else {
      if (transform.position.x < _diePositionX) {
        Destroy(gameObject);
      }
    }

    transform.position += (Vector3)(_moveDirection * _movementSpeed * Time.deltaTime);
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    
  }

  public void Die() {
    Alive = false;

    _dieParticleSytem.Play();
    _spriteRenderer.DOFade(0, .45f).SetDelay(1.05f - .45f).OnComplete(() => Destroy(gameObject));
  }
}
