using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameObject Head;
    public float MouseSensitivity = 0.1f;

    [HideInInspector]
    public CharacterController Controller;

    [HideInInspector]
    public StateMachine<Player> StateMachine;

    [HideInInspector]
    public float Gravity = -9.81f;

    public Vector3 Velocity = new();

    [HideInInspector]
    public InputActions.PlayerActions Input;

    public bool Running { get => Input.running.IsPressed(); }
    public bool Jumping { get => Input.jumping.IsPressed(); }
    public Vector2 Looking { get => Input.looking.ReadValue<Vector2>(); }
    public Vector2 Moving { get => Input.moving.ReadValue<Vector2>(); }


    void Awake()
    {
        Input = new InputActions().player;

        StateMachine = new StateMachine<Player>(new StandingState(this))
            .With(new RunningState(this))
            .With(new FallingState(this));

        Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateLooking();

        if (Controller.isGrounded)
        {

            if (Running && Moving.y >= 0)
            {
                UpdateMoving(0.05f);
                return;
            }

            UpdateMoving(0.02f);
            return;
        }

        UpdateFalling(0.015f);
    }

    void OnEnable()
    {
        Input.Enable();
    }

    void OnDisable()
    {
        Input.Disable();
    }


    private void UpdateFalling(float Speed)
    {

        var direction = (transform.rotation * new Vector3(Moving.x, 0f, Moving.y)).normalized;

        var horizontalVelocity = new Vector3(Velocity.x, 0f, Velocity.z);

        var speed = Speed * Time.deltaTime;

        var velocity = horizontalVelocity + direction * speed;

        if (horizontalVelocity.magnitude > speed)
        {
            var coefficient = horizontalVelocity.Angle(direction) / 180;
            velocity = velocity.normalized
            * (horizontalVelocity.magnitude - speed * (float)coefficient);
        }

        velocity.y = Velocity.y + Gravity * Mathf.Pow(Time.deltaTime, 2);

        Velocity = velocity;
        Controller.Move(velocity);
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

    private void UpdateMoving(float Speed)
    {
        var acceleration = 10;

        var horizontalVelocity = new Vector3(Velocity.x, 0.0f, Velocity.z);

        var direction = (transform.rotation * new Vector3(Moving.x, 0.0f, Moving.y)).normalized;

        var velocity = horizontalVelocity.Lerp(direction * Speed, acceleration * Time.deltaTime);

        velocity.y = Velocity.y + Gravity * Mathf.Pow(Time.deltaTime, 2);

        Velocity = velocity;
        Controller.Move(velocity);
    }

}


