using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class ToggleFlip : MonoBehaviour
{
    [Header("Flip")]
    public Button Button;

    [Header("Objects")]
    public GameObject InitialObject;
    public GameObject SecondObject;

    private bool isFacingRight = true;


    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        if (InitialObject.activeSelf)
        {
            InitialObject.SetActive(false);
            SecondObject.SetActive(true);
        }
        else
        {
            InitialObject.SetActive(true);
            SecondObject.SetActive(false);
        }
    }
    public void ResetButton()
    {
        InitialObject.SetActive(true);
        SecondObject.SetActive(false);
        isFacingRight = true;
        transform.localScale = Vector3.one;
    }
}
