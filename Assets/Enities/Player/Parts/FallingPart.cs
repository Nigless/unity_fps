

using System;
using ExtensionMethods;
using UnityEngine;

public class FallingPart : PartMachine<Player>.Part
{
    private float _Speed;

    public FallingPart(Player Context, float Speed) : base(Context)
    {
        _Speed = Speed;
    }

    public override void Update()
    {

        var direction = (_Context.transform.rotation * new Vector3(_Context.Moving.x, 0f, _Context.Moving.y)).normalized;

        var horizontalVelocity = new Vector3(_Context.Velocity.x, 0f, _Context.Velocity.z);

        var speed = _Speed * Time.deltaTime;

        var velocity = horizontalVelocity + direction * speed;

        if (horizontalVelocity.magnitude > _Speed)
        {
            var coefficient = horizontalVelocity.Angle(direction) / 180;
            velocity = velocity.normalized
            * (horizontalVelocity.magnitude - speed * (float)coefficient);
        }

        velocity.y = _Context.Velocity.y + _Context.Gravity * Mathf.Pow(Time.deltaTime, 2);

        _Context.Velocity = velocity;
        _Context.Controller.Move(velocity);
    }

}