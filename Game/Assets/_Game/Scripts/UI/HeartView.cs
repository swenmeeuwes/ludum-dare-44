using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeartView : MonoBehaviour
{
  [SerializeField] private Sprite[] _states; // should be Length = 5

  private Image _image;

  private int _state;
  public int State {
    get => _state;
    set {
      if (value >= _states.Length) {
        Debug.LogWarning("Cannot set heart view to out of bounds state");
        return;
      }

      _state = value;
      _image.sprite = _states[value];
    }
  }

  public int HealthPointsAbleToShow => _states.Length - 1; // -1 because first state = empty

  private void Awake() {
    _image = GetComponent<Image>();
  }
}
