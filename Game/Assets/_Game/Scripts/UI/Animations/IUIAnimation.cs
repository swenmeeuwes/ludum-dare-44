using RSG;
using UnityEngine;

public interface IUIAnimation {
    IPromise AnimateOn(RectTransform transform);
}