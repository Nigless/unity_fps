

using System;
using ExtensionMethods;
using UnityEngine;

public class MovingPart : PartMachine<Player>.Part
{

    private float _Speed;
    private float _Acceleration = 1;
    private float _JumpHigh = 0.05f;

    public MovingPart(Player Context, float Speed) : base(Context)
    {
        _Speed = Speed;
    }

    public override void Update()
    {
        var horizontalVelocity = new Vector3(_Context.Velocity.x, 0.0f, _Context.Velocity.z);

        var direction = (_Context.transform.rotation * new Vector3(_Context.Moving.x, 0.0f, _Context.Moving.y)).normalized;

        var velocity = horizontalVelocity.Lerp(direction * _Speed, _Acceleration * Time.deltaTime);

        velocity.y = _Context.Velocity.y + _Context.Gravity * Mathf.Pow(Time.deltaTime, 2);

        _Context.Velocity = velocity;
        _Context.Controller.Move(velocity);
    }

}