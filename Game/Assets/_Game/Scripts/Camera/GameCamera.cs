using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GameCamera : MonoBehaviour
{
  public static GameCamera Instance { get; set; } // todo: inject instead of singleton

  private Camera _camera;
  private Vector3 _originalPosition;

  private void Awake() {
    Instance = this;

    _camera = GetComponent<Camera>();
    _originalPosition = transform.position;
  }

  public void Shake() {
    _camera.DOShakePosition(.1f, strength: .1f).OnComplete(() => transform.position = _originalPosition);
  }
}
