using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public GameObject Head;
    public float MouseSensitivity = 0.1f;
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
    public float SlopeLimit = 1f;
    public float SkinWidth = 0.01f;

    private PlayerInput Input;
    private Rigidbody Body;
    private CapsuleCollider Collider;
    private Vector3 Gravity = Vector3.down * 9.81f;
    public Vector3 GroundSurface = Vector3.zero;
    private float DistanceToGround = 0;
    private float DistanceToCelling = 0;
    private bool CanStandUp => DistanceToGround + DistanceToCelling + Collider.radius * 2 > StandingHeight;

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        Body = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.Dot(Body.velocity.normalized) >= 0)
            return;

        Body.velocity = Vector3.ProjectOnPlane(Body.velocity, hit.normal);
    }

    private void Update()
    {
        UpdateCelling();
        UpdateGround();
        UpdateLooking();

        if (Input.Crouching)
            UpdateCollider(CrouchingHeight);
        else if (CanStandUp) UpdateCollider(StandingHeight);

        if (GroundSurface != Vector3.zero)
        {
            if (Input.Crouching || !CanStandUp)
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
        }
        else
        {
            if (Input.Moving != Vector2.zero)
                UpdateFalling(FallingSpeed);
        }
    }

    private void UpdateFalling(float speed)
    {
        var direction = (transform.rotation * new Vector3(Input.Moving.x, 0f, Input.Moving.y)).normalized;

        var horizontalVelocity = Vector3.ProjectOnPlane(Body.velocity, Gravity);

        var verticalVelocity = Body.velocity - horizontalVelocity;

        var acceleration = (((direction * speed) - horizontalVelocity).normalized.Dot(direction) + 1) / 2 * FallingAcceleration;

        horizontalVelocity = horizontalVelocity.MoveTowards(direction * speed, acceleration);

        Body.velocity = horizontalVelocity + verticalVelocity;
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
        Body.velocity = Body.velocity.ProjectOnPlane(GroundSurface) + GroundSurface * jumpHigh;
        gameObject.SendMessage("OnJump", null, SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateMoving(float speed)
    {
        var direction = Quaternion.FromToRotation(Vector3.up, GroundSurface)
           * transform.rotation
           * new Vector3(Input.Moving.x, 0.0f, Input.Moving.y).normalized;

        var horizontalVelocity = Vector3.ProjectOnPlane(Body.velocity, GroundSurface);

        var verticalVelocity = Body.velocity - horizontalVelocity;

        horizontalVelocity = horizontalVelocity.MoveTowards(direction * speed, Acceleration);

        Body.velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateCollider(float height)
    {

        if (Collider.height == height)
        {
            return;
        }

        var transition = CrouchingTransition * Time.deltaTime;

        var headPosition = Head.transform.localPosition;

        headPosition.y = headPosition.y.Lerp(height / 2 - Collider.radius, transition);

        var currentHeight = Collider.height;

        Collider.height = currentHeight.Lerp(height, transition);

        if (DistanceToGround < currentHeight / 2 - Collider.radius + SkinWidth)
        {
            var heightDiff = (Collider.height - currentHeight) / 2;
            transform.position += Vector3.up * heightDiff;
        }

        Head.transform.localPosition = headPosition;
    }

    private void UpdateCelling()
    {
        var distance = StandingHeight - Collider.height / 2 - Collider.radius + SkinWidth;

        RaycastHit hit;

        if (Physics.SphereCast(
              transform.position,
              Collider.radius,
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
        var distance = StandingHeight - Collider.height / 2 - Collider.radius + SkinWidth;

        RaycastHit hit;

        if (!Physics.SphereCast(
              transform.position,
              Collider.radius,
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
            hit.distance < Collider.height / 2 - Collider.radius + 0.1f
            && Vector3.Angle(hit.normal, Vector3.up) < SlopeLimit
            )
        {
            GroundSurface = hit.normal;
            return;
        }

        GroundSurface = Vector3.zero;

    }

}


