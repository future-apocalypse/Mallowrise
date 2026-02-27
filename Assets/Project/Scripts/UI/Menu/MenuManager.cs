using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuJump _marshmallow;
    [SerializeField] private float _delayBeforeLoad = 4f;

    private bool _started;

    private void Awake()
     {
        Application.targetFrameRate = 60;
     }

    public void StartGame()
    {
        if (_started) return;
        _started = true;

        _marshmallow.JumpForward();
        StartCoroutine(LoadGameAfterDelay());
    }

    private IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(_delayBeforeLoad);
        SceneManager.LoadScene("Game");
    }
}
