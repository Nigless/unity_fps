using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    public GameObject Head;
    public float RunningSpeed = 0.05f;
    public float WalkingSpeed = 0.02f;
    public float FallingSpeed = 0.015f;
    public float CrouchingSpeed = 0.015f;

    public float StandingJumpHeight = 0.02f;
    public float CrouchingJumpHeight = 0.02f;

    public float GroundedAcceleration = 10;
    public float FallingAcceleration = 3;

    public float ColliderStandingHeight = 1.7f;
    public float ColliderCrouchingHeight = 1f;
    public float ColliderTransitionSpeed = 1f;
    public float SlopeAngleLimit = 1f;
    public float SkinWidth = 0.01f;

    private PlayerInput PlayerInput;
    private Rigidbody RigidBody;
    private CapsuleCollider Collider;
    private int LayerMask;

    private Vector3 _GroundSurface = Vector3.zero;
    public Vector3 GroundSurface { get { return _GroundSurface; } }

    private GameObject _StandingOn;
    public GameObject StandingOn { get { return _StandingOn; } }

    private float DistanceToGround = 0;
    private float DistanceToCelling = 0;
    private bool CanStandUp => DistanceToGround + DistanceToCelling > ColliderStandingHeight;

    private void Reset()
    {
        var collider = GetComponent<CapsuleCollider>();
        collider.height = ColliderStandingHeight;
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        RigidBody = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();

        LayerMask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
            {
                LayerMask |= 1 << i;
            }
        }

        LayerMask &= ~(1 << gameObject.layer);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        UpdateGravity();

        if (_GroundSurface == Vector3.zero)
        {
            if (PlayerInput.Moving != Vector2.zero)
                UpdateFalling();

            return;
        }


        if (PlayerInput.Crouching || !CanStandUp)
        {
            UpdateMoving(CrouchingSpeed);

            if (PlayerInput.Jumping) UpdateJumping(CrouchingJumpHeight);

            return;
        }

        if (PlayerInput.Running)
            UpdateMoving(RunningSpeed);
        else
            UpdateMoving(WalkingSpeed);

        if (PlayerInput.Jumping) UpdateJumping(StandingJumpHeight);
    }

    private void Update()
    {
        UpdateCelling();
        UpdateGround();

        UpdateLooking();

        if (PlayerInput.Crouching)
            UpdateCollider(ColliderCrouchingHeight);
        else if (CanStandUp) UpdateCollider(ColliderStandingHeight);
    }

    private void UpdateGravity()
    {
        RigidBody.velocity += Physics.gravity * Time.deltaTime;
    }

    private void UpdateFalling()
    {
        var direction = (transform.rotation * new Vector3(PlayerInput.Moving.x, 0f, PlayerInput.Moving.y)).normalized;

        var horizontalVelocity = Vector3.ProjectOnPlane(RigidBody.velocity, Physics.gravity);

        var verticalVelocity = RigidBody.velocity - horizontalVelocity;

        var acceleration = ((direction * FallingSpeed) - horizontalVelocity).normalized.Dot(direction);

        acceleration = (acceleration + 1) / 2;

        acceleration *= FallingAcceleration * Time.deltaTime;

        horizontalVelocity = horizontalVelocity.Lerp(direction * FallingSpeed, acceleration);

        RigidBody.velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateLooking()
    {
        var rotation = Head.transform.localRotation * Quaternion.Euler(-PlayerInput.Looking.y, 0f, 0f);

        if ((rotation * Vector3.up).y < 0)
        {
            if ((rotation * Vector3.forward).y >= 0)
                rotation = Quaternion.Euler(-90f, 0f, 0f);
            else rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        Head.transform.localRotation = rotation;

        transform.Rotate(Vector3.up * PlayerInput.Looking.x);
    }

    private void UpdateJumping(float jumpHigh)
    {
        RigidBody.velocity = RigidBody.velocity.ProjectOnPlane(_GroundSurface) + _GroundSurface * jumpHigh;
        gameObject.SendMessage("OnJump", null, SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateMoving(float speed)
    {
        var direction = Quaternion.FromToRotation(Vector3.up, _GroundSurface)
           * transform.rotation
           * new Vector3(PlayerInput.Moving.x, 0.0f, PlayerInput.Moving.y).normalized;

        var horizontalVelocity = Vector3.ProjectOnPlane(RigidBody.velocity, _GroundSurface);

        var verticalVelocity = RigidBody.velocity - horizontalVelocity;

        horizontalVelocity = horizontalVelocity.Lerp(direction * speed, GroundedAcceleration * Time.deltaTime);

        RigidBody.velocity = horizontalVelocity + verticalVelocity;
    }

    private void UpdateCollider(float height)
    {
        if (Collider.height == height)
        {
            return;
        }

        var transition = ColliderTransitionSpeed * Time.deltaTime;

        var headPosition = Head.transform.localPosition;

        headPosition.y = headPosition.y.Lerp(height / 2 - Collider.radius, transition);

        var currentHeight = Collider.height;

        Collider.height = currentHeight.Lerp(height, transition);

        Head.transform.localPosition = headPosition;

        if (DistanceToGround > Collider.height / 2 && !(GroundSurface != Vector3.zero && Collider.height < currentHeight))
            return;

        var heightDiff = (Collider.height - currentHeight) / 2;
        transform.position += Vector3.up * heightDiff;
    }

    private void UpdateCelling()
    {
        var distance = ColliderStandingHeight - Collider.height / 2 - Collider.radius + SkinWidth * 2;

        RaycastHit hit;

        if (Physics.SphereCast(
              transform.position,
              Collider.radius - SkinWidth,
              transform.up,
              out hit,
              distance,
            LayerMask
          ))
        {
            DistanceToCelling = hit.distance + Collider.radius - SkinWidth;

            return;
        }

        DistanceToCelling = float.PositiveInfinity;
    }

    private void UpdateGround()
    {
        var distance = ColliderStandingHeight - Collider.height / 2 - Collider.radius + SkinWidth * 2;

        RaycastHit hit;

        if (!Physics.SphereCast(
            transform.position,
            Collider.radius - SkinWidth,
            -transform.up,
            out hit,
            distance
        ))
        {
            DistanceToGround = float.PositiveInfinity;
            _GroundSurface = Vector3.zero;
            return;
        }

        _StandingOn = hit.collider.gameObject;

        DistanceToGround = hit.distance + Collider.radius - SkinWidth;

        if (DistanceToGround > Collider.height / 2 + SkinWidth)
        {
            _GroundSurface = Vector3.zero;
            return;
        }

        if (Vector3.Angle(hit.normal, Vector3.up) > SlopeAngleLimit)
        {
            _GroundSurface = Vector3.zero;
            return;
        }

        _GroundSurface = hit.normal;
    }

}


