using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _gameOverClip;

    [SerializeField] private AudioClip[] _startGameSound;
    
    
    [SerializeField] float minJumpPitch = 0.95f;
    [SerializeField] float maxJumpPitch = 1.05f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayJumpSound()
    {
        if(_jumpClip == null) return;
        
        _audioSource.pitch = Random.Range(minJumpPitch, maxJumpPitch);
        _audioSource.PlayOneShot(_jumpClip);
        _audioSource.pitch = 1f;
    }

    public void PlayGameOverSound()
    {
        if (_gameOverClip == null) return;
        _audioSource.pitch = 1f;
        _audioSource.PlayOneShot(_gameOverClip);
    }
    
    public void PlayStartGameSound()
    {
        if (_startGameSound.Length == 0) return;
        AudioClip clip = GetRandomClip(_startGameSound);
        
        _audioSource.pitch = 1f;
        _audioSource.PlayOneShot(clip);
    }
    
    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
