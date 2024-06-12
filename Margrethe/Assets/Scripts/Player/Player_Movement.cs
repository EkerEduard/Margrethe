using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    private Player player;

    private PlayerControlls controlls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement info")]
    private float speed; // Скорость по умолчанию
    [SerializeField] private float walkSpeed; // Скорость ходьбы
    [SerializeField] private float runSpeed; // Скорость бега
    [SerializeField] private float gravityScale = 9.81f;
    private Vector3 movementDirection;
    private float verticalVelocity;
    private bool isRunning; //Состояние бега

    [Header("Aim info")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    public Vector3 lookingDirection;

    private Vector2 moveInput;
    private Vector2 aimInput;

    private void Start()
    {
        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void AimTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hitInfo.point - transform.position;
            lookingDirection.y = 0.0f;
            lookingDirection.Normalize();

            transform.forward = lookingDirection;

            aim.position = new Vector3(hitInfo.point.x, transform.position.y + 1.15f, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -0.5f;
        }
    }

    private void AssignInputEvents()
    {
        controlls = player.controlls;

        controlls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controlls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controlls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controlls.Character.Aim.canceled += controls => aimInput = Vector2.zero;

        controlls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        controlls.Character.Run.canceled += controls =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }
}
