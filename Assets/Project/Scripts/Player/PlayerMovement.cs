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
    
    private float _rotationSpeed = 15f;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        
        Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y);
            
        if(movement.sqrMagnitude > 1)
              movement.Normalize();
        
        if (movement.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.fixedDeltaTime
            );
        }


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
