using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : SeekingEnemy {
  private void Start() {
    Target = GameObject.Find("Player").transform; // TEMP TEST
  }
}
