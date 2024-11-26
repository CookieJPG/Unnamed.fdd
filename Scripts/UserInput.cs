using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class UserInput : MonoBehaviour
{
    private InputActionReference move, jump, attack, dash; 
    [SerializeField] private InputActionReference escape;

    [Header("References")]
    [SerializeField] private InputActionReference moveRef;
    [SerializeField] private InputActionReference jumpRef;
    [SerializeField] private InputActionReference attackRef;
    [SerializeField] private InputActionReference dashRef;

    public Vector2 moveInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool jumpReleaseInput { get; private set; }
    public bool attackInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool escapeInput { get; private set; }

    internal void SetControlScheme(string playerIdentity)
    {
        move = moveRef;
        jump = jumpRef;
        attack = attackRef;
        dash = dashRef;
    }

    private void Update() 
    {
        moveInput = move.action.ReadValue<Vector2>();
        jumpInput = jump.action.ReadValue<float>() > 0;
        jumpReleaseInput = jump.action.WasReleasedThisFrame();
        attackInput = attack.action.ReadValue<float>() > 0;
        dashInput = dash.action.ReadValue<float>() > 0;
        escapeInput = escape.action.ReadValue<float>() > 0;
    }
}
