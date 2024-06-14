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
    [SerializeField] private float walkSpeed; // Скорость ходьбы
    [SerializeField] private float runSpeed; // Скорость бега
    [SerializeField] private float turnSpeed; // Скорость поворота
    [SerializeField] private float gravityScale = 9.81f; // Гравитация для игрока
    private float speed; // Скорость по умолчанию
    private float verticalVelocity;

    private Vector3 movementDirection;
    public Vector2 moveInput {  get; private set; }

    private bool isRunning; //Состояние бега


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
        ApplyRotation();
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

    private void ApplyRotation()
    {
        Vector3 lookingDirection = player.aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0.0f;
        lookingDirection.Normalize();

        //Плавное вращение игрока
        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
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
