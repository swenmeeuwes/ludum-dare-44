using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerMovement : MonoBehaviour {
  [SerializeField] private float _fallMultiplier = 2.5f;
  [SerializeField] private float _lowJumpMultiplier = 2f;
  [SerializeField] private float _jumpForce;
  [SerializeField] private float _movementSpeed = 365f;
  [SerializeField] private float _maxMovementSpeed = 5f;

  [SerializeField] private CollisionCheck _groundCheck;
  [SerializeField] private ParticleSystem _jumpParticleSystem;

  private Rigidbody2D _rigidbody;
  private SpriteRenderer _renderer;
  private Animator _animator;

  public float AddedMovementSpeed { get; set; }
  public float AddedMaxMovementSpeed { get; set; }
  public float AddedJumpForce { get; set; }

  public float MovementSpeed { get => _movementSpeed + AddedMovementSpeed; }
  public float MaxMovementSpeed { get => _maxMovementSpeed + AddedMaxMovementSpeed; }
  public float JumpForce { get => _jumpForce + AddedJumpForce; }

  public bool CanMove { get; set; }
  public bool QueueStopMoving { get; set; }

  private bool _queueJump;

  private void Start() {
    _rigidbody = GetComponent<Rigidbody2D>();
    _renderer = GetComponent<SpriteRenderer>();
    _animator = GetComponent<Animator>();
  }

  private void Update() {
    // Update the animator
    _animator.SetFloat("Speed", Mathf.Abs(_rigidbody.velocity.x));
    _animator.SetBool("IsGrounded", _groundCheck.IsColliding);

    if (!CanMove) {
      return;
    }

    var input = new Vector2(Input.GetAxis(InputAxes.Horizontal), Input.GetAxis(InputAxes.Vertical));

    if (Input.GetButton(InputAxes.Jump)) {
      _queueJump = true;
    }

    if (QueueStopMoving && _groundCheck.IsColliding && _rigidbody.velocity.magnitude < .01f) {
      QueueStopMoving = false;
      CanMove = false;
    }
  }

  private void FixedUpdate() {
    if (!CanMove) {
      return;
    }

    var input = new Vector2(Input.GetAxis(InputAxes.Horizontal), Input.GetAxis(InputAxes.Vertical));

    //if (input.x * _rigidbody.velocity.x < MaxMovementSpeed) {
    if (Mathf.Abs(input.x) > 0.1f) {
      _rigidbody.AddForce(Vector2.right * input.x * MovementSpeed);
    }

    if (Mathf.Abs(_rigidbody.velocity.x) > MaxMovementSpeed) {
      _rigidbody.velocity = new Vector2(Mathf.Sign(_rigidbody.velocity.x) * MaxMovementSpeed, _rigidbody.velocity.y);
    }

    if (Mathf.Abs(input.x) > 0.05f) {
      _renderer.flipX = input.x > 0 ? false : true;
    }

    // Jumping
    if (_queueJump) {
      if (_groundCheck.IsColliding && Input.GetButton(InputAxes.Jump)) {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
        //_rigidbody.AddForce(Vector2.up * JumpForce);

        _jumpParticleSystem.Play();
      }

      // Make the jumping nicer
      if (_rigidbody.velocity.y < 0) {
        _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
      }
      else if (_rigidbody.velocity.y > 0 && !Input.GetButton(InputAxes.Jump)) {
        _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
      }

      _queueJump = false; // Handled jump
    }
  }
}
