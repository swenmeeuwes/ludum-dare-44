using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour {
  [SerializeField] private List<GameObject> _platforms;

  private List<PlatformInfo> _platformInfoList;

  public int AmountOfEnabledPlatforms { get => _platformInfoList.Count(p => p.Enabled); }
  private List<PlatformInfo> EnabledPlatforms { get => _platformInfoList.Where(p => p.Enabled).ToList(); }
  private List<PlatformInfo> DisabledPlatforms { get => _platformInfoList.Where(p => !p.Enabled).ToList(); }

  private void Awake() {
    _platformInfoList = _platforms.Select(p => new PlatformInfo {
      Platform = p,
      OriginalPosition = p.transform.position,
      Enabled = true
    }).ToList();
  }

  public void SetAmountOfEnabledPlatforms(int amount) {
    var amountOfPlatformsToEnable = amount - AmountOfEnabledPlatforms;
    if (amountOfPlatformsToEnable < 0) {
      DisableRandomPlatforms(Mathf.Abs(amountOfPlatformsToEnable));
    }
    else if (amountOfPlatformsToEnable > 0) {
      EnableRandomPlatforms(Mathf.Abs(amountOfPlatformsToEnable));
    }
  }

  private void DisableRandomPlatforms(int amount) {
    for (int i = 0; i < amount; i++) {
      if (EnabledPlatforms.Count == 0) {
        break;
      }

      var randomEnabledPlatform = EnabledPlatforms[Random.Range(0, EnabledPlatforms.Count)];
      randomEnabledPlatform.Enabled = false;

      var platform = randomEnabledPlatform.Platform.transform;
      var platformDisappearDelay = 2f;
      platform.DOShakePosition(platformDisappearDelay + .85f, strength: .1f, vibrato: 20);
      DOTween.Sequence()
        .SetDelay(platformDisappearDelay)
        .Append(platform.DOMoveY(-Camera.main.orthographicSize - 5, .85f))
        .OnComplete(() => {
          platform.gameObject.SetActive(false);
        });
    }
  }

  private void EnableRandomPlatforms(int amount) {
    for (int i = 0; i < amount; i++) {
      if (DisabledPlatforms.Count == 0) {
        break;
      }

      var randomDisabledPlatform = DisabledPlatforms[Random.Range(0, DisabledPlatforms.Count)];
      randomDisabledPlatform.Enabled = true;

      var platform = randomDisabledPlatform.Platform.transform;
      platform.gameObject.SetActive(true);
      platform.DOShakePosition(.85f, strength: .1f, vibrato: 20);
      platform.DOMoveY(randomDisabledPlatform.OriginalPosition.y, .85f);
    }
  }

  private class PlatformInfo {
    public GameObject Platform;
    public Vector3 OriginalPosition;
    public bool Enabled;
  }
}
