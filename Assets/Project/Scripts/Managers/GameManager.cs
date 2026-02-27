using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
 public static GameManager Instance;
 
 public int jumpCount;
 public float survivalTime;
 

 [SerializeField] private float baseSinkTime = 5f;
 [SerializeField] private float baseSpawnInterval = 3f;
 [SerializeField] private int maxDifficultyTier = 5;
 private int _difficultyTier;

 
 [SerializeField] private TextMeshProUGUI marshmallowCountText;
 private int _marshmallowCount = 0;
 
 [SerializeField] private GameObject pausePanel;
 private bool _isPaused;
 
 [SerializeField] private GameObject countdownPanel;
 [SerializeField] private TextMeshProUGUI countdownText;
 private bool _isGameRunning;
 
 [SerializeField] private GameObject gameOverPanel;
 
 [SerializeField] private GameObject pauseButton;
 
 private void Awake()
 {
  Application.targetFrameRate = 60;
  
  
  if (Instance == null)
      Instance = this;
  else {
      Destroy(gameObject);
      
  }
 }

 private void Start()
 {
  StartCoroutine(StartCountdown());
 }

 private void Update()
 {
  survivalTime -= Time.deltaTime;
 }
 public void RegisterJump()
 {
  jumpCount++;

  int calculatedTier = Mathf.Min(jumpCount / 10, maxDifficultyTier);

  if (calculatedTier > _difficultyTier)
  {
   _difficultyTier = calculatedTier;
   ApplyDifficulty();
  }
 }

 private void ApplyDifficulty()
 {
  float sinkTime = Mathf.Clamp(baseSinkTime - _difficultyTier * 1f, 1f, baseSinkTime);
  float spawnInterval = Mathf.Clamp(baseSpawnInterval - _difficultyTier * 0.3f, 0.5f, baseSpawnInterval);
  
  PlatformManager.Instance.SetSinkTime(sinkTime);
  Spawner.Instance.SetSpawnInterval(spawnInterval);
 }

 public void PlayerDied()
 {
  gameOverPanel.SetActive(true);
  Time.timeScale = 0f;
  _isGameRunning = false;
  PlayerMovement.Instance.DisableInput();
  AudioManager.Instance.PlayGameOverSound();
  pausePanel.SetActive(false);
  pauseButton.SetActive(false);
  
 }

 private void ReloadScene()
 {
  SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
 }
 public void AddMarshmallow()
 {
  _marshmallowCount++;
  UpdateUI();
 }

 private void UpdateUI()
 {
  marshmallowCountText.text = _marshmallowCount.ToString();
 }
 public void ResetScore()
 {
  _marshmallowCount = 0;
  UpdateUI();
 }

 public void ToglePause()
 {
  if (_isPaused)
  {
   Resume();
  }
  else
  {
   Pause();
  }
 }

 public void Pause()
 {
  _isPaused = true;
  pausePanel.SetActive(true);
  Time.timeScale = 0f;
  AudioListener.pause = true;

  _isGameRunning = false;
  PlayerMovement.Instance.DisableInput();
 }

 public void ResumeWithCountdown()
 {
  StartCoroutine(ResumeCountdown());
 }

 private IEnumerator StartCountdown()
 {
  Time.timeScale = 0f;
  _isGameRunning = false;
  countdownPanel.SetActive(true);
  
  yield return RunCountdown();
  countdownPanel.SetActive(false);
  Time.timeScale = 1f;
  _isGameRunning = true;
 }

 private IEnumerator ResumeCountdown()
 {
  Time.timeScale = 0f;
  AudioListener.pause = false;

  countdownPanel.SetActive(true);

  yield return RunCountdown();

  countdownPanel.SetActive(false);
  Time.timeScale = 1f;
  _isGameRunning = true;
 }

 private IEnumerator RunCountdown()
 {
  countdownText.text = "3";
  yield return new WaitForSecondsRealtime(0.7f);

  countdownText.text = "2";
  yield return new WaitForSecondsRealtime(0.6f);

  countdownText.text = "1";
  yield return new WaitForSecondsRealtime(0.5f);

  countdownText.text = "GO!";
  yield return new WaitForSecondsRealtime(0.3f);
 }

 public void Resume()
 {
  _isPaused = false;
  pausePanel.SetActive(false);

  StartCoroutine(ResumeCountdown());
  PlayerMovement.Instance.EnableInput();
 }

 public void Restart()
 {
  Time.timeScale = 1f;
  SceneManager.LoadScene(0);
  PlayerMovement.Instance.EnableInput();
  AudioListener.pause = false;
 }

}
