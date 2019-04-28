using DG.Tweening;
using RSG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurtainController : MonoBehaviour {
  [SerializeField] private Animator _curtainAnimator;
  [SerializeField] private TMP_Text _text;
  [SerializeField] private CanvasGroup _curtainCanvas;

  private void Awake() {
    _curtainCanvas.DOFade(0, 0);
    _text.DOFade(0, 0);
  }

  public IPromise ShowEnteringRoundText(string roundText) {
    var promise = new Promise();

    StartCoroutine(ShowEnteringRoundTextSequence(promise, roundText));

    return promise;
  }

  private IEnumerator ShowEnteringRoundTextSequence(Promise promise, string roundText) {
    _curtainCanvas.DOFade(0, 0);
    _text.DOFade(0, 0);

    yield return new WaitForSeconds(.65f);

    _curtainAnimator.SetTrigger("Close");

    yield return new WaitForSeconds(1f);

    _curtainCanvas.DOFade(1, .35f);

    yield return new WaitForSeconds(.35f);

    _text.text = roundText;
    _text.DOFade(1, .45f);

    yield return new WaitForSeconds(1.65f);

    _text.DOFade(0, .35f);
    _curtainCanvas.DOFade(0, .45f);

    yield return new WaitForSeconds(.5f);

    _curtainAnimator.SetTrigger("Open");

    yield return new WaitForSeconds(1f);

    promise.Resolve();
  }
}
