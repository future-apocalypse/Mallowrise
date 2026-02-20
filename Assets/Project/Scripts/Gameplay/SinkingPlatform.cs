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
    
    [SerializeField] private float _riseOffset = 1.2f;
    [SerializeField] private float _riseDuration = 0.6f;
    //[SerializeField] private float _riseForce = 8f;

    private bool _isRising;
    
    void Start()
    {
        
        //_restY = transform.position.y;
    }

    void FixedUpdate()
    {
        if (_rb == null || _isRising) return;
        
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
    private System.Collections.IEnumerator RiseRoutine()
    {
        float startY = transform.position.y;
        float t = 0f;

        while (t < _riseDuration)
        {
            t += Time.deltaTime;
            float progress = t / _riseDuration;

            float smooth = Mathf.SmoothStep(0f, 1f, progress);
            float newY = Mathf.Lerp(startY, _restY, smooth);

            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );

            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            _restY,
            transform.position.z
        );

        _rb.isKinematic = false;
        _isRising = false;
    }
    public void Initialize(float surfaceY)
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();
        
        _restY = surfaceY;

        Vector3 pos = transform.position;
        pos.y = surfaceY - _riseOffset;
        transform.position = pos;

        _rb.isKinematic = true;
        _isRising = true;

        StartCoroutine(RiseRoutine());
    }
    
}
