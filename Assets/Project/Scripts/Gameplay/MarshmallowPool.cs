using System.Collections.Generic;
using UnityEngine;

public class MarshmallowPool : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int poolSizePerPrefab = 5;

    private Dictionary<GameObject, Queue<SinkingPlatform>> _pools
        = new Dictionary<GameObject, Queue<SinkingPlatform>>();

    public static MarshmallowPool Instance;

    private void Awake()
    {
        Instance = this;

        foreach (var prefab in prefabs)
        {
            Queue<SinkingPlatform> queue = new Queue<SinkingPlatform>();

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);

                SinkingPlatform platform = obj.GetComponent<SinkingPlatform>();
                platform.SetOriginalPrefab(prefab);

                queue.Enqueue(platform);
            }

            _pools.Add(prefab, queue);
        }
    }

    public SinkingPlatform Get(GameObject prefab)
    {
        var queue = _pools[prefab];

        SinkingPlatform platform;

        if (queue.Count > 0)
        {
            platform = queue.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(prefab, transform);
            platform = obj.GetComponent<SinkingPlatform>();
            platform.SetOriginalPrefab(prefab);
        }

        platform.gameObject.SetActive(true);
        platform.OnReturnToPool = ReturnToPool;

        return platform;
    }

    private void ReturnToPool(SinkingPlatform platform)
    {
        platform.gameObject.SetActive(false);

        var queue = _pools[platform.OriginalPrefab];
        queue.Enqueue(platform);
    }
}