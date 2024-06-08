using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private PlayerControlls controls;

    public Vector2 moveInput;
    public Vector2 aimInput;

    private void Awake()
    {
        controls = new PlayerControlls();

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += controls => aimInput = Vector2.zero;
    }

    // Acitve
    private void OnEnable()
    {
        controls.Enable();
    }


    // Inactive
    private void OnDisable()
    {
        controls.Disable();
    }
}
