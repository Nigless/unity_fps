using Unity.VisualScripting;
using UnityEngine;

class PlayerInput : MonoBehaviour
{

    private Vector2 _Moving;
    public Vector2 Moving { get { return _Moving; } }

    private Vector2 _Looking;
    public Vector2 Looking { get { return _Looking; } }

    private bool _Crouching;
    public bool Crouching { get { return _Crouching; } }

    private bool _Running;
    public bool Running { get { return _Running && _Moving.y >= 0; } }

    private bool _Jumping;
    public bool Jumping { get { return _Jumping; } }

    public float JumpCooldown = 2f;
    private float LastPressed = -Mathf.Infinity;


    public InputActions.PlayerActions Input;

    private void OnEnable()
    {
        Input = new InputActions().player;

        Input.running.started += (context) => _Running = true;
        Input.running.canceled += (context) => _Running = false;

        Input.crouching.started += (context) => _Crouching = true;
        Input.crouching.canceled += (context) => _Crouching = false;

        Input.jumping.started += (context) =>
        {
            LastPressed = Time.time;
            _Jumping = true;
        };

        Input.moving.performed += (context) => _Moving = context.ReadValue<Vector2>();
        Input.moving.canceled += (context) => _Moving = Vector2.zero;

        Input.looking.performed += (context) => _Looking = context.ReadValue<Vector2>();
        Input.looking.canceled += (context) => _Looking = Vector2.zero;

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