using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject Head;
    public float MouseSensitivity = 0.1f;
    public float SlideSlopeLimit = 45;
    public float RunningSpeed = 0.05f;
    public float WalkingSpeed = 0.02f;
    public float FallingSpeed = 0.015f;
    public float CrouchingSpeed = 0.015f;
    public float JumpHeight = 0.02f;
    public float Acceleration = 10;
    public float StandingHeight = 1.7f;
    public float CrouchingHeight = 1f;
    public float CrouchingJumpHeight = 0.02f;
    public float CrouchingTransition = 1f;

    private float Gravity = -9.81f;
    private Vector3 Surface = Vector3.zero;
    public Vector3 Velocity = new();
    private CharacterController Controller;
    private InputActions.PlayerActions Input;
    private bool Running;
    private bool Jumping;
    private bool Crouching;
    private Vector2 Looking { get => Input.looking.ReadValue<Vector2>(); }
    private Vector2 Moving { get => Input.moving.ReadValue<Vector2>(); }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        Input = new InputActions().player;

        Input.running.started += (context) => Running = true;
        Input.running.canceled += (context) => Running = false;

        Input.crouching.started += (context) => Crouching = true;
        Input.crouching.canceled += (context) => Crouching = false;

        Input.jumping.started += (context) => Jumping = true;

        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Velocity = Vector3.ProjectOnPlane(Velocity, hit.normal);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (Surface != Vector3.zero)
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + Vector3.down * (Controller.height / 2 - Controller.radius + 0.1f), Controller.radius);
    }

    private void Update()
    {
        UpdateGrounded();
        UpdateLooking();

        if (Crouching)
            UpdateCollider(CrouchingHeight);
        else UpdateCollider(StandingHeight);

        if (Surface != Vector3.zero)
        {
            if (Crouching)
            {
                UpdateMoving(CrouchingSpeed);

                if (Jumping) UpdateJumping(CrouchingJumpHeight);
            }
            else
            {
                if (Running && Moving.y >= 0)
                    UpdateMoving(RunningSpeed);
                else
                    UpdateMoving(WalkingSpeed);

                if (Jumping) UpdateJumping(JumpHeight);
            }

            if (Vector3.Angle(Surface, Vector3.up) > SlideSlopeLimit)
                UpdateGravity();

        }
        else
        {
            if (Moving != Vector2.zero)
                UpdateFalling(FallingSpeed);

            UpdateGravity();
        }

        UpdatePosition();

        Jumping = false;
    }

    private void UpdateFalling(float speed)
    {
        var direction = (transform.rotation * new Vector3(Moving.x, 0f, Moving.y)).normalized;

        var horizontalVelocity = new Vector3(Velocity.x, 0f, Velocity.z);

        var verticalVelocity = Vector3.up * Velocity.y;

        var acceleration = (((direction * speed) - horizontalVelocity).normalized.Dot(direction) + 1) / 2;

        horizontalVelocity = horizontalVelocity.Lerp(direction * speed, acceleration * Time.deltaTime);

        Velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateLooking()
    {
        var rotation = Head.transform.localRotation * Quaternion.Euler(-Looking.y * MouseSensitivity, 0f, 0f);

        if ((rotation * Vector3.up).y < 0)
        {
            if ((rotation * Vector3.forward).y >= 0)
                rotation = Quaternion.Euler(-90f, 0f, 0f);
            else rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        Head.transform.localRotation = rotation;

        transform.Rotate(Vector3.up * Looking.x * MouseSensitivity);
    }

    private void UpdateJumping(float jumpHigh)
    {
        Velocity += Surface * jumpHigh;
    }

    private void UpdateMoving(float speed)
    {
        var direction = Vector3.ProjectOnPlane((transform.rotation * new Vector3(Moving.x, 0.0f, Moving.y)).normalized, Surface);

        var horizontalVelocity = Vector3.ProjectOnPlane(Velocity, Surface);

        var verticalVelocity = Velocity - horizontalVelocity;

        horizontalVelocity = horizontalVelocity.Lerp(direction * speed, Acceleration * Time.deltaTime);

        Velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateGravity()
    {
        Velocity.y += (float)(Gravity * Math.Pow(Time.deltaTime, 2));
    }

    private void UpdatePosition()
    {
        Controller.Move(Velocity);
    }

    private void UpdateCollider(float height)
    {
        var currentHeight = Controller.height;

        var position = Head.transform.localPosition;

        var transition = CrouchingTransition * Time.deltaTime;

        if (Controller.height == height)
        {
            return;
        }

        position.y = position.y.Lerp(height / 2 - Controller.radius, transition);
        Controller.height = currentHeight.Lerp(height, transition);

        if (Surface != Vector3.zero)
        {
            var heightDiff = (Controller.height - currentHeight) / 2;

            Controller.Move(Vector3.up * heightDiff);
        }

        Head.transform.localPosition = position;
    }

    private void UpdateGrounded()
    {
        var collisions = Physics.SphereCastAll(
            transform.position,
            Controller.radius,
            -transform.up,
            Controller.height / 2 - Controller.radius + 0.1f
        );

        foreach (var hit in collisions)
        {
            if (Vector3.Angle(hit.normal, Vector3.up) <= Controller.slopeLimit)
            {
                Surface = hit.normal;
                return;
            }
        }

        Surface = Vector3.zero;
    }

}


