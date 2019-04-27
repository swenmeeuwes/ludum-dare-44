using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRoot : MonoBehaviour
{
    [SerializeField]
    private Canvas _canvas;

    public Canvas Canvas { get { return _canvas; } }
}
