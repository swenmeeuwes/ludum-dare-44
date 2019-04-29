using DG.Tweening;
using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertArrowManager : MonoBehaviour
{
  [SerializeField] private GameObject _alertArrowContainer;
  [SerializeField] private GameObject _alertArrowPrefab;
  [SerializeField] private float _alertArrowTopSpacing = -130f;
  [SerializeField] private float _arrowLifeSpanInSeconds = 3f;

  public IPromise ShowAlertArrowAtWorldX(float worldPositionX) {
    var promise = new Promise();

    var arrow = CreateArrow();
    var arrowPositionX = Camera.main.WorldToScreenPoint(new Vector3(worldPositionX, 0, 0)).x;
    var arrowPosition = arrow.position;
    arrowPosition.x = arrowPositionX;

    arrow.transform.position = arrowPosition;

    var arrowImage = arrow.GetComponent<Image>();
    DOTween.Sequence()
      .Append(arrowImage.DOFade(0, 0))
      .Append(arrowImage.DOFade(1, .35f))
      .Append(arrowImage.DOFade(1, _arrowLifeSpanInSeconds)) // Stay visible for X seconds
      .Append(arrowImage.DOFade(0, .25f))
      .OnComplete(() => promise.Resolve());

    return promise;
  }

  private Transform CreateArrow() {
    var alertArrow = Instantiate(_alertArrowPrefab).transform;
    alertArrow.SetParent(_alertArrowContainer.transform, false);

    var alertArrowPosition = alertArrow.position;
    alertArrowPosition.y = _alertArrowTopSpacing;
    alertArrow.position = alertArrowPosition;

    return alertArrow;
  }
}
