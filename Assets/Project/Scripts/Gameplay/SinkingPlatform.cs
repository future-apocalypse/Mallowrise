using UnityEngine;

public class SinkingPlatform : MonoBehaviour
{
    [SerializeField] private float _springStrength = 40f;
    [SerializeField] private float _damping = 6f;
    [SerializeField] private float _playerWeightForce = 10f;
    [SerializeField] private float _maxSinkDepth = 0.4f;

    [SerializeField] private float _sinkDelay = 3f;
    [SerializeField] private float _defaultSinkDelay = 3;
    [SerializeField] private float _finalSinkSpeed = 0.5f;
    [SerializeField] private float _destroyY = -5f;
    
    
    private Rigidbody _rb;
    private float _restY;
    private bool _playerOnPlatform;

    private float _timer;
    private bool _sinkingForever;
    private bool _timerActive;

    private bool _hasBeenCounted = false;
    
    [SerializeField] private ParticleSystem _splashParticles;
    private bool _triggered = false;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _restY = transform.position.y;
    }

    void FixedUpdate()
    {
        if (_sinkingForever)
        {
            _rb.linearDamping = 1f;
            _rb.AddForce(Vector3.down * _finalSinkSpeed, ForceMode.VelocityChange);
            return;
            
        }

        float displacement = _restY - _rb.position.y;
        float springForce = displacement * _springStrength;
        float dampingForce = -_rb.linearVelocity.y * _damping;

        _rb.AddForce(Vector3.up * (springForce + dampingForce), ForceMode.Force);

        _rb.linearDamping = _playerOnPlatform ? 4f : 2f;

        if (_playerOnPlatform)
        {
            _rb.AddForce(Vector3.down * _playerWeightForce, ForceMode.Force);
            
        }

        float minY = _restY - _maxSinkDepth;
        if (_rb.position.y < minY)
        {
            _rb.position = new Vector3(_rb.position.x, minY, _rb.position.z);
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        }
    }

    void Update()
    {
        if (!_sinkingForever && _timerActive)
        {
            _timer += Time.deltaTime;

            if (_timer >= _sinkDelay)
            {
                _sinkingForever = true;
                _rb.linearVelocity = Vector3.zero;
            }
        }

        if (transform.position.y < _destroyY)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasBeenCounted) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = true;
            _timerActive = true;
            
            _sinkDelay = PlatformManager.Instance != null
                ? PlatformManager.Instance.GetSinkTime()
                : _defaultSinkDelay;
            
            GameManager.Instance.RegisterJump();

            _hasBeenCounted = true;
            GameManager.Instance.AddMarshmallow();

            if (_triggered) return;
            _triggered = true;
            _splashParticles.Play();

        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            _playerOnPlatform = false;
            
        }
    }
}
