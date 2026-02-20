using UnityEngine;
using System.Collections;

public class MarshmallowPlatform : MonoBehaviour
{
    [Header("Rise Settings")]
    [SerializeField] private float riseHeightOffset = 1.2f;
    [SerializeField] private float riseDuration = 0.6f;
    [SerializeField] private float overshootAmount = 0.15f;

    [Header("Sink Settings")]
    [SerializeField] private float sinkSpeed = 1.5f;

    private float _targetY;
    private Collider _collider;

    public void Initialize(float surfaceY)
    {
        _targetY = surfaceY;
        _collider = GetComponent<Collider>();
        _collider.enabled = false;

        Vector3 pos = transform.position;
        pos.y = surfaceY - riseHeightOffset;
        transform.position = pos;

        StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        float startY = transform.position.y;
        float overshootY = _targetY + overshootAmount;

        float t = 0f;

        // Rise
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float progress = t / riseDuration;

            float smooth = Mathf.SmoothStep(0f, 1f, progress);
            float newY = Mathf.Lerp(startY, overshootY, smooth);

            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );

            yield return null;
        }

        // Settle back softly
        t = 0f;
        float settleDuration = 0.2f;

        while (t < settleDuration)
        {
            t += Time.deltaTime;
            float progress = t / settleDuration;

            float newY = Mathf.Lerp(overshootY, _targetY, progress);

            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );

            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            _targetY,
            transform.position.z
        );

        _collider.enabled = true;
    }

    public void Sink()
    {
        StartCoroutine(SinkRoutine());
    }

    private IEnumerator SinkRoutine()
    {
        while (transform.position.y > _targetY - 3f)
        {
            transform.position += Vector3.down * sinkSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}