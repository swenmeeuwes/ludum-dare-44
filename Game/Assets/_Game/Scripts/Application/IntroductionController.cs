﻿using DG.Tweening;
using RSG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionController : MonoBehaviour
{
  [SerializeField] private Animator _curtainAnimator;
  [SerializeField] private TMP_Text _welcomeText;
  [SerializeField] private CanvasGroup _introductionCanvas;
  [SerializeField] private CanvasGroup _tutorialCanvas;
  [SerializeField] private Image _figure;
  [SerializeField] private string[] _introTexts;
  [SerializeField] private string[] _welcomeBackTexts;

  //private void Start() {
  //  //StartCoroutine(Sequence());
  //  FinishSequence(); // for testing
  //}

  public IPromise PlayIntro() {
    var promise = new Promise();

    StartCoroutine(Sequence(promise));

    // For testing
    //FinishSequence(promise);
    //promise.Then(() => { _curtainAnimator.speed = 1; }); // reset speed

    return promise;
  }

  private IEnumerator Sequence(Promise promise) {
    _introductionCanvas.DOFade(0, 0);
    _welcomeText.DOFade(0, 0);
    _tutorialCanvas.DOFade(0, 0);

    _introductionCanvas.DOFade(1, .45f);

    var textsToShow = _welcomeBackTexts;
    if (PlayerPrefs.GetInt(PlayerPrefKey.IntroSeen, 0) == 0) {
      textsToShow = _introTexts;
      PlayerPrefs.SetInt(PlayerPrefKey.IntroSeen, 1);
    }

    foreach (var introText in textsToShow) {
      yield return new WaitForSeconds(.45f);

      _welcomeText.text = introText;
      _welcomeText.DOFade(1f, .65f);

      yield return new WaitForSeconds(1f + .08f * introText.Length);

      _welcomeText.DOFade(0, .85f);

      yield return new WaitForSeconds(1f);
    }

    if (PlayerPrefs.GetInt(PlayerPrefKey.TutorialSeen, 0) == 0) {
      _figure.DOFade(0, .45f);

      yield return new WaitForSeconds(1f);

      _tutorialCanvas.DOFade(1, .45f);

      yield return new WaitForSeconds(6f);

      _tutorialCanvas.DOFade(0, .35f);

      PlayerPrefs.SetInt(PlayerPrefKey.TutorialSeen, 1);
    }

    yield return new WaitForSeconds(.15f);

    _introductionCanvas.DOFade(0, .85f);

    yield return new WaitForSeconds(1f);

    _curtainAnimator.SetTrigger("Open");

    yield return new WaitForSeconds(1.5f);

    promise.Resolve();
  }

  private void FinishSequence(Promise promise) {
    _introductionCanvas.DOFade(0, 0);

    _curtainAnimator.speed = 10f;
    _curtainAnimator.SetTrigger("Open");

    promise.Resolve();
  }
}
