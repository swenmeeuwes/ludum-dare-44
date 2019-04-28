using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HealthView : MonoBehaviour
{
  protected HeartView HeartViewPrefab;

  protected HorizontalLayoutGroup LayoutGroup;

  protected readonly List<HeartView> HeartViews = new List<HeartView>();

  [Inject]
  private void Construct(HeartView heartView) {
    HeartViewPrefab = heartView;
  }

  private void Awake() {
    LayoutGroup = GetComponent<HorizontalLayoutGroup>();
  }

  public void ShowDamageTaken() {
    foreach (var heartView in HeartViews) {
      if (heartView.State > 0) {
        heartView.transform.DOPunchScale(Vector3.one, .05f);
      }
    }
  }

  public void ShowHealth(int amount) {
    var healthPerHeart = HeartViewPrefab.HealthPointsAbleToShow;
    var healthToShowLeft = amount;
    foreach (var heartView in HeartViews) {
      if (healthToShowLeft <= 0) {
        heartView.State = 0;
        continue;
      }

      heartView.State = Mathf.Clamp(healthToShowLeft, 0, healthPerHeart);
      healthToShowLeft -= healthPerHeart;
    }
  }

  public void EnsureEnoughHeartsFor(int health) {
    ShowAmountOfHearts(Mathf.CeilToInt((float)health / HeartViewPrefab.HealthPointsAbleToShow));
  }

  public void ShowAmountOfHearts(int maxHearts) {
    var heartsToAdd = maxHearts - HeartViews.Count;
    if (heartsToAdd > 0) {
      AddHearts(heartsToAdd);
    } else if (heartsToAdd < 0) {
      RemoveHearts(heartsToAdd);
    }
  }

  private void AddHearts(int amount) {
    for (int i = 0; i < amount; i++) {
      var heartView = GameObject.Instantiate(HeartViewPrefab);
      heartView.transform.SetParent(LayoutGroup.transform, false);

      DOTween.Sequence()
        .PrependInterval(i * .15f)
        .Append(heartView.transform.DOPunchScale(Vector3.one, .15f));

      HeartViews.Add(heartView);
    }
  }

  private void RemoveHearts(int amount) {
    throw new NotImplementedException();
  }
}
