using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
 public int jumpCount;
 public float survivalTime;
 public static GameManager Instance;

 [SerializeField] private float baseSinkTime = 5f;
 [SerializeField] private float baseSpawnInterval = 3f;
 [SerializeField] private int maxDifficultyTier = 5;
 private int _difficultyTier;

 
 [SerializeField] private TextMeshProUGUI _marshmallowCountText;
 private int _marshmallowCount = 0;
 
 private void Awake()
 {
  if (Instance == null)
      Instance = this;
  else {
      Destroy(gameObject);
      
  }
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
  Invoke(nameof(ReloadScene), 0.7f);
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
  _marshmallowCountText.text = _marshmallowCount.ToString();
 }
 public void ResetScore()
 {
  _marshmallowCount = 0;
  UpdateUI();
 }

}
