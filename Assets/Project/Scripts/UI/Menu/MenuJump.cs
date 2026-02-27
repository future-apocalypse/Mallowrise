using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MenuJump : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _hasJumped;

    [SerializeField] private float forwardForce = 6f;
    [SerializeField] private float upwardForce = 5f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    public void JumpForward()
    {
        if (_hasJumped) return;

        _hasJumped = true;

        Vector3 impulse = transform.forward * forwardForce + Vector3.up * upwardForce;
        _rb.AddForce(impulse, ForceMode.VelocityChange);

        AudioManager.Instance.PlayStartGameSound();
    }
    
    private void Update()
    {
        if (!_hasJumped)
        {
            float floatAmount = Mathf.Sin(Time.time * 2f) * 0.05f;
            transform.position += Vector3.up * floatAmount * Time.deltaTime;
        }
    }
}

