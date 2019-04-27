using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroductionController : MonoBehaviour
{
  [SerializeField] private Animator _curtainAnimator;
  [SerializeField] private TMP_Text _welcomeText;
  [SerializeField] private CanvasGroup _introductionCanvas;

  private void Start() {
    StartCoroutine(Sequence());
  }

  private IEnumerator Sequence() {
    _introductionCanvas.DOFade(0, 0);
    _welcomeText.DOFade(0, 0);

    _introductionCanvas.DOFade(1, .45f);

    yield return new WaitForSeconds(.45f);

    _welcomeText.DOFade(1f, .65f);

    yield return new WaitForSeconds(1f);

    _introductionCanvas.DOFade(0, .85f);

    yield return new WaitForSeconds(1f);

    _curtainAnimator.SetTrigger("Open");
  }
}
