using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private float _moveSpeed = 5.0f;
    private float _groundDistance = 1.5f;
    private float _jumpForce = 7.0f;
    
    [SerializeField]private LayerMask whatIsGround;
    private bool _jumpRequest;
    private Vector2 _moveInput;
    
    [SerializeField] private VirtualJoystick joystick;
    //public Vector2 InputVector { get; private set; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 joystickInput = joystick != null ? joystick.inputVector : Vector2.zero;

        if (joystickInput.sqrMagnitude > 0.01f)
        {
            _moveInput = joystickInput;
        }
        
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y);
        if(movement.sqrMagnitude > 1)
            movement.Normalize();
            
        Vector3 horizontalVelocity = movement * _moveSpeed;
        _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);

        if (IsGrounded() && _jumpRequest)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpForce, _rb.linearVelocity.z);
        }
        _jumpRequest = false;
        
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(joystick != null && joystick.inputVector.sqrMagnitude > 0.01f)
            return;
        
        _moveInput = context.ReadValue<Vector2>();
    }

   public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _jumpRequest = true;
        }
    }
    public void RequestJump()
    {
        _jumpRequest = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundDistance, whatIsGround);
        
    }
}
