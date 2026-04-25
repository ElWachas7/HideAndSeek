using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState { Menu, Playing, Paused, Won, Lost, Resumed}
    [SerializeField] private GameState currentState;
    public bool IsAlive => isAlive;
    private bool isAlive;
    public event Action OnGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                Debug.Log("Menu");
                break;
            case GameState.Playing:
                isAlive = true;
                Time.timeScale = 1f;
                Debug.Log("Playing");
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                Debug.Log("Paused");
                break;
            case GameState.Won:
                Time.timeScale = 0f;
                Debug.Log("Won");
                break;
            case GameState.Lost:
                Time.timeScale = 0f;
                Debug.Log("Lost");
                break;
            case GameState.Resumed:
                Debug.Log("Resumed");
                break;
        }
    }

    public void MainMenu()
    {
        ChangeState(GameState.Menu);
    }

    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    public void WinGame()
    {
        ChangeState(GameState.Won);
    }

    public void LoseGame()
    {
        OnGameOver.Invoke();
        ChangeState(GameState.Lost);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Resumed);
    }

    public void CheckState()
    {
        if(!isAlive)
        {
            LoseGame();
        }
    }
}