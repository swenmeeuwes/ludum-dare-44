using DG.Tweening;
using RSG;
using System;
using UnityEngine;

public class UISlideAnimation : IUIAnimation
{
    private Direction _direction;
    private float _durationInSeconds;
    private bool _withFade;

    public UISlideAnimation(Direction direction, float durationInSeconds, bool withFade = false)
    {
        _direction = direction;
        _durationInSeconds = durationInSeconds;
        _withFade = withFade;
    }

    public IPromise AnimateOn(RectTransform transform)
    {
        var promise = new Promise();
        Animate(transform, () => promise.Resolve());

        return promise;
    }

    private void Animate(RectTransform transform, Action callback)
    {
        var screenHeight = UnityEngine.Screen.height;
        var yCorrection = 0;
        if (transform.pivot.y == 0.5f) // todo: unsafe comparision?
        {
            yCorrection = screenHeight / 2;
        }

        switch (_direction)
        {
            case Direction.FROM_TOP:
                transform.DOMoveY(screenHeight + yCorrection, 0);
                transform.DOMoveY(yCorrection, _durationInSeconds).OnComplete(() => callback.Invoke());
                break;
            case Direction.TO_TOP:
                transform.DOMoveY(yCorrection, 0);
                transform.DOMoveY(screenHeight + yCorrection, _durationInSeconds).OnComplete(() => callback.Invoke());
                break;
            default:
                throw new NotImplementedException();
        }

        if (_withFade)
        {
            var canvasGroup = transform.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                var fromAlpha = 1;
                var toAlpha = 0;
                if ((int)_direction < 4) // todo: fix assumption that Direction < 4 = an in animation
                {
                    fromAlpha = 0;
                    toAlpha = 1;
                }

                canvasGroup.DOFade(fromAlpha, 0);
                canvasGroup.DOFade(toAlpha, _durationInSeconds);
            }
        }
    }

    public enum Direction
    {
        FROM_TOP = 0,
        FROM_BOTTOM = 1,
        FROM_LEFT = 2,
        FROM_RIGHT = 3,

        TO_TOP = 4,
        TO_BOTTOM = 5,
        TO_LEFT = 6,
        TO_RIGHT = 7
    }
}
