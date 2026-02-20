using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;
    [SerializeField] private float currentSinkTime = 5f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        Instance = this;
    }

    public void SetSinkTime(float value)
    {
        currentSinkTime = value;
    }
    public float GetSinkTime()
    {
        return currentSinkTime;
    }
}
