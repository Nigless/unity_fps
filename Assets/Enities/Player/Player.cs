using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
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
    public float FallingAcceleration = 3;
    public float StandingHeight = 1.7f;
    public float CrouchingHeight = 1f;
    public float CrouchingJumpHeight = 0.02f;
    public float CrouchingTransition = 1f;

    private PlayerInput Input;
    private CharacterController Controller;
    private float Gravity = -9.81f;
    private Vector3 GroundSurface = Vector3.zero;
    private float DistanceToGround = 0;
    private float DistanceToCelling = 0;
    private Vector3 Velocity = new();
    private bool Crouching => Input.Crouching || DistanceToGround + DistanceToCelling + Controller.radius * 2 + Controller.skinWidth < StandingHeight;

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.Dot(Velocity.normalized) >= 0)
            return;

        Velocity = Vector3.ProjectOnPlane(Velocity, hit.normal);
    }

    private void Update()
    {
        UpdateCelling();
        UpdateGround();
        UpdateLooking();

        if (Crouching)
            UpdateCollider(CrouchingHeight);
        else UpdateCollider(StandingHeight);

        if (GroundSurface != Vector3.zero)
        {
            if (Crouching)
            {
                UpdateMoving(CrouchingSpeed);

                if (Input.Jumping) UpdateJumping(CrouchingJumpHeight);
            }
            else
            {
                if (Input.Running)
                    UpdateMoving(RunningSpeed);
                else
                    UpdateMoving(WalkingSpeed);

                if (Input.Jumping) UpdateJumping(JumpHeight);
            }

            if (Input.Jumping)
                Input.OnJump();
            else if (Vector3.Angle(GroundSurface, Vector3.up) > SlideSlopeLimit)
                UpdateGravity();

        }
        else
        {
            if (Input.Moving != Vector2.zero)
                UpdateFalling(FallingSpeed);

            UpdateGravity();
        }

        UpdatePosition();
    }

    private void UpdateFalling(float speed)
    {
        var direction = (transform.rotation * new Vector3(Input.Moving.x, 0f, Input.Moving.y)).normalized;

        var horizontalVelocity = new Vector3(Velocity.x, 0f, Velocity.z);

        var verticalVelocity = Vector3.up * Velocity.y;

        var acceleration = (((direction * speed) - horizontalVelocity).normalized.Dot(direction) + 1) / 2 * FallingAcceleration;

        horizontalVelocity = horizontalVelocity.MoveTowards(direction * speed, acceleration * Time.deltaTime);

        Velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateLooking()
    {
        var rotation = Head.transform.localRotation * Quaternion.Euler(-Input.Looking.y * MouseSensitivity, 0f, 0f);

        if ((rotation * Vector3.up).y < 0)
        {
            if ((rotation * Vector3.forward).y >= 0)
                rotation = Quaternion.Euler(-90f, 0f, 0f);
            else rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        Head.transform.localRotation = rotation;

        transform.Rotate(Vector3.up * Input.Looking.x * MouseSensitivity);
    }

    private void UpdateJumping(float jumpHigh)
    {
        Velocity += GroundSurface * jumpHigh;
    }

    private void UpdateMoving(float speed)
    {
        var direction = Vector3.ProjectOnPlane((transform.rotation * new Vector3(Input.Moving.x, 0.0f, Input.Moving.y)).normalized, GroundSurface);

        var horizontalVelocity = Vector3.ProjectOnPlane(Velocity, GroundSurface);

        var verticalVelocity = Velocity - horizontalVelocity;

        horizontalVelocity = horizontalVelocity.MoveTowards(direction * speed, Acceleration * Time.deltaTime);

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

        if (Controller.height == height)
        {
            return;
        }

        var transition = CrouchingTransition * Time.deltaTime;

        var headPosition = Head.transform.localPosition;

        headPosition.y = headPosition.y.Lerp(height / 2 - Controller.radius, transition);

        var currentHeight = Controller.height;

        Controller.height = currentHeight.Lerp(height, transition);

        if (DistanceToGround < Controller.height / 2 - Controller.radius + Controller.skinWidth + 0.1f)
        {
            var heightDiff = (Controller.height - currentHeight) / 2;
            Controller.minMoveDistance = 0;
            Controller.Move(Vector3.up * heightDiff);
            Controller.minMoveDistance = 0.001f;
        }

        Head.transform.localPosition = headPosition;
    }

    private void UpdateCelling()
    {
        var distance = StandingHeight - Controller.height / 2 - Controller.radius + Controller.skinWidth + 0.1f;

        RaycastHit hit;

        if (Physics.SphereCast(
              transform.position,
              Controller.radius,
              transform.up,
              out hit,
              distance
          ))
        {
            DistanceToCelling = hit.distance;

            return;
        }

        DistanceToCelling = float.PositiveInfinity;
    }

    private void UpdateGround()
    {
        var distance = StandingHeight - Controller.height / 2 - Controller.radius + Controller.skinWidth + 0.1f;

        RaycastHit hit;

        if (!Physics.SphereCast(
              transform.position,
              Controller.radius,
              -transform.up,
              out hit,
              distance
          ))
        {
            DistanceToGround = float.PositiveInfinity;
            GroundSurface = Vector3.zero;
            return;
        }

        DistanceToGround = hit.distance;

        if (
            hit.distance < Controller.height / 2 - Controller.radius + 0.1f
            && Vector3.Angle(hit.normal, Vector3.up) < Controller.slopeLimit
            )
        {
            GroundSurface = hit.normal;
            return;
        }

        GroundSurface = Vector3.zero;

    }

}


