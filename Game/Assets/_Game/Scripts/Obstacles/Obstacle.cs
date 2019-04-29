using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour {
  protected Collider2D Collider;
  protected Rigidbody2D Rigidbody;

  protected virtual void Awake() {
    Collider = GetComponent<Collider2D>();
    Rigidbody = GetComponent<Rigidbody2D>();
  }
}
