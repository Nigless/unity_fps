using System;
using Unity.VisualScripting;
using UnityEngine;

class PlayerInput : MonoBehaviour
{
    public float MouseSensitivity = 0.1f;

    private Vector2 _Moving;
    public Vector2 Moving { get { return _Moving; } }

    private Vector2 _Looking;
    public Vector2 Looking { get { return _Looking * MouseSensitivity; } }

    private bool _Crouching;
    public bool Crouching { get { return _Crouching; } }

    private bool _Running;
    public bool Running { get { return _Running && _Moving.y >= 0; } }

    private bool _Grabbing;
    public bool Grabbing { get { return _Grabbing; } }
    public Action GrabbingStarted;
    public Action GrabbingCancelled;

    public Action ThrowingStarted;

    private bool _Picking;
    public bool Picking { get { return _Picking; } }
    public Action PickingStarted;
    public Action PickingCanceled;

    public Action DroppingStarted;

    public Action<float> LiftingStarted;

    private bool _Jumping;
    public bool Jumping { get { return _Jumping; } }

    public float JumpCooldown = 2f;
    private float LastPressed = -Mathf.Infinity;

    public InputActions.PlayerActions Input;

    private void OnEnable()
    {
        Input = new InputActions().player;

        Input.running.started += (_) => _Running = true;
        Input.running.canceled += (_) => _Running = false;

        Input.grabbing.started += (_) => _Grabbing = true;
        Input.grabbing.canceled += (_) => _Grabbing = false;
        Input.grabbing.started += (_) => GrabbingStarted?.Invoke();
        Input.grabbing.canceled += (_) => GrabbingCancelled?.Invoke();

        Input.picking.started += (_) => _Picking = true;
        Input.picking.canceled += (_) => _Picking = false;
        Input.picking.started += (_) => PickingStarted?.Invoke();
        Input.picking.canceled += (_) => PickingCanceled?.Invoke();

        Input.dropping.started += (_) => DroppingStarted?.Invoke();

        Input.throwing.started += (_) => ThrowingStarted?.Invoke();

        Input.lifting.started += (Context) => LiftingStarted?.Invoke(Context.ReadValue<float>());

        Input.crouching.started += (_) => _Crouching = true;
        Input.crouching.canceled += (_) => _Crouching = false;

        Input.jumping.started += (_) =>
        {
            LastPressed = Time.time;
            _Jumping = true;
        };

        Input.moving.performed += (context) => _Moving = context.ReadValue<Vector2>();
        Input.moving.canceled += (_) => _Moving = Vector2.zero;

        Input.looking.performed += (context) => _Looking = context.ReadValue<Vector2>();
        Input.looking.canceled += (_) => _Looking = Vector2.zero;

        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    public void OnJump()
    {
        _Jumping = false;
    }

    private void Update()
    {
        if (_Jumping && Time.time - LastPressed >= JumpCooldown)
            _Jumping = false;
    }

}