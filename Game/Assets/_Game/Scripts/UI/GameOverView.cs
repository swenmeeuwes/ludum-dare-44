using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverView : MonoBehaviour {
  [SerializeField] private TMP_Text _levelReachTextField;
  [SerializeField] private TMP_Text _scoreTextField;
  [SerializeField] private TMP_Text _highScoreTextField;
  [SerializeField] private TMP_Text _newHighScoreTextField;
  [SerializeField] private CanvasGroup _retryButton;

  [Inject] private ScoreController _scoreController;
  [Inject] private LevelController _levelController;

  public CanvasGroup CanvasGroup { get; set; }

  private void Awake() {
    CanvasGroup = GetComponent<CanvasGroup>();
  }

  public void Hide() {
    _levelReachTextField.DOFade(0, 0);
    _scoreTextField.DOFade(0, 0);
    _highScoreTextField.DOFade(0, 0);
    _newHighScoreTextField.DOFade(0, 0);
    //_retryButton.DOFade(0, 0);
  }

  public void Show() {
    Hide();

    var newHighScore = _scoreController.SaveHighScore();

    _levelReachTextField.text = "Level reached: " + (_levelController.CurrentLevelIndex + 1).ToString();
    _scoreTextField.text = "Score: " + _scoreController.Score;
    _highScoreTextField.text = "High Score: " + _scoreController.HighScore;

    var showSequence = DOTween.Sequence()
      .Append(_levelReachTextField.DOFade(1, .45f))
      .Append(_scoreTextField.DOFade(1, .45f))
      .Append(_highScoreTextField.DOFade(1, .45f));

    if (newHighScore) {
      showSequence.Append(DOTween.Sequence()
        .Append(_newHighScoreTextField.DOFade(1, .45f))
        .Append(_newHighScoreTextField.transform.DOPunchScale(Vector3.one, .45f)));
    }

    showSequence
      .SetDelay(.65f)
      .Append(_retryButton.DOFade(1, .45f))
      .OnComplete(() => {
        _retryButton.alpha = 1;
        _retryButton.interactable = true;
      });
  }
}
