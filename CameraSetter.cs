using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    public Camera MainCamera;

    void Start()
    {
        Invoke(nameof(SetCamTarget), 0.1f);
    }

    internal void SetCamTarget()
    {
        GameObject character = GetComponent<LoadCharacter>().character;
        CamFollow cam = MainCamera.GetComponent<CamFollow>();
        cam.enabled = true;
        cam.target = character.transform;
    }
}
