using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
 [SerializeField] private GameObject[] _deadMarshmallowPrefab;
 
 [SerializeField] private float _spawnInterval = 3f;
 [SerializeField] private BoxCollider cupBounds;

 [SerializeField] private float _minHorizontalDistance = 2f;
 [SerializeField] private float _maxHorizontalDistance = 5f;
 [SerializeField] private float _horizontalPadding = 1f;
 [SerializeField] private int _maxActivePlatforms = 4;
 
 private Vector3 _lastSpawnPosition;
 private bool _hasLastSpawnPosition;
 
 
 private Coroutine _spawnRoutine;
 
 public static Spawner Instance;

 private void Awake()
 {
     Instance = this;
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
        if (transform.childCount >= _maxActivePlatforms)
            return;
        int randomIndex = Random.Range(0, _deadMarshmallowPrefab.Length);
        GameObject selected = _deadMarshmallowPrefab[randomIndex];
        
        Vector3 spawnPosition = GetValidSpawnPosition();
        Quaternion baseRotation = selected.transform.rotation;
        Quaternion randomY = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Instantiate(selected, spawnPosition, baseRotation * randomY, transform);}
    
    

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
