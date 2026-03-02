using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    private List<SinkingPlatform> _activePlatforms = new List<SinkingPlatform>();
 [SerializeField] private GameObject[] _deadMarshmallowPrefab;
 
 [SerializeField] private float _spawnInterval = 3f;
 [SerializeField] private BoxCollider cupBounds;

 [SerializeField] private float _minHorizontalDistance = 2f;
 [SerializeField] private float _maxHorizontalDistance = 5f;
 [SerializeField] private float _horizontalPadding = 1f;
 [SerializeField] private int _maxActivePlatforms = 3;
 
 private Vector3 _lastSpawnPosition;
 private bool _hasLastSpawnPosition;
 
 
 private Coroutine _spawnRoutine;
 
 public static Spawner Instance;
 
 private MarshmallowPool _pool;

 private void Awake()
 {
     Instance = this;
     _pool = MarshmallowPool.Instance;
 }
 
    void Start()
    {
        StartSpawning();
       
        
    }

    private void StartSpawning()
    {
        if (_spawnRoutine == null)
            _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine); 
        _spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnMarshmallow();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = cupBounds.bounds;
        
        float minX = bounds.min.x + _horizontalPadding;
        float maxX = bounds.max.x - _horizontalPadding;

        float minZ = bounds.min.z + _horizontalPadding;
        float maxZ = bounds.max.z - _horizontalPadding;

        return new Vector3(
            Random.Range(minX, maxX),
            bounds.max.y,
            Random.Range(minZ, maxZ)
        );
    }
    
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 candidate;
        int attempts = 0;

        do
        {
            candidate = GetRandomPosition();
            attempts++;

            if (!_hasLastSpawnPosition)
                break;

            float distance = Vector3.Distance(
                new Vector3(candidate.x, 0f, candidate.z),
                new Vector3(_lastSpawnPosition.x, 0f, _lastSpawnPosition.z)
            );

            if (distance >= _minHorizontalDistance && distance <= _maxHorizontalDistance)
                break;

        } while (attempts < 10);

        _lastSpawnPosition = candidate;
        _hasLastSpawnPosition = true;

        return candidate;
    }
    private void SpawnMarshmallow()
    {
        if (_activePlatforms.Count >= _maxActivePlatforms)
            return;

        int randomIndex = Random.Range(0, _deadMarshmallowPrefab.Length);
        GameObject selected = _deadMarshmallowPrefab[randomIndex];

        Vector3 spawnPosition = GetValidSpawnPosition();

        int attempts = 0;
        while (_activePlatforms.Any(p =>
                   Vector3.Distance(new Vector3(p.transform.position.x, 0, p.transform.position.z),
                       new Vector3(spawnPosition.x, 0, spawnPosition.z))
                   < _minHorizontalDistance) && attempts < 10)
        {
            spawnPosition = GetValidSpawnPosition();
            attempts++;
        }

        Quaternion baseRotation = selected.transform.rotation;
        Quaternion randomY = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        SinkingPlatform platform = _pool.Get(selected);
        
        platform.transform.SetPositionAndRotation(
            spawnPosition,
            baseRotation * randomY
        );
        
        if (platform != null)
        {
            platform.Initialize(spawnPosition.y);
            _activePlatforms.Add(platform);
            platform.OnDestroyed += () => _activePlatforms.Remove(platform);
        }
    }
    
    public void SetSpawnInterval(float value)
    {
        _spawnInterval = value;
        
        StopSpawning();
        StartSpawning();
    }
    public float GetSpawnInterval()
    {
        return _spawnInterval;
    }
    
}
