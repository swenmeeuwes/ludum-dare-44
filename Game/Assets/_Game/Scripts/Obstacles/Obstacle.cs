using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour {
  [SerializeField] private int _damage;

  public int Damage { get => _damage; }

  protected Collider2D Collider;
  protected Rigidbody2D Rigidbody;

  protected virtual void Awake() {
    Collider = GetComponent<Collider2D>();
    Rigidbody = GetComponent<Rigidbody2D>();
  }
}
