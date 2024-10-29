using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToogler : MonoBehaviour
{
    [Header("UI")]
    public Canvas Canvas;

    public void ToogleCanvas()
    {
        Canvas.gameObject.SetActive(!Canvas.isActiveAndEnabled);
    }
}
