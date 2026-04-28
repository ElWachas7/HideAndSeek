using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<MyPath> paths = new List<MyPath>();
    public static GameManager Instance;
    public enum GameState { Menu, Playing, Paused, Won, Lost, Resumed }
    [SerializeField] private GameState currentState;
    public GameState CurrentState => currentState;
    public bool IsAlive => isAlive;
    private bool isAlive;
    public event Action OnGameOver;
    public event Action OnGameWin;

    public List<HidingEnemy> hidingEnemies;
    public List<HidingSpot> hidingSpots;

    public int Points => points;
    private int points = 5;
    public bool IsPaused => isPaused;
    private bool isPaused = false;

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
    private void Start()
    {
        ChangeState(GameState.Menu);
        ResetHidingSpots();
    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                Debug.Log("Menu");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
                isAlive = true;
                Time.timeScale = 1f;
                Debug.Log("Playing");
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                Debug.Log("Paused");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Won:
                Time.timeScale = 0f;
                Debug.Log("Won");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Lost:
                Time.timeScale = 0f;
                Debug.Log("Lost");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Resumed:
                Time.timeScale = 1f;
                Debug.Log("Resumed");
                Time.timeScale = 1f;
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void StartGame()
    {
        ChangeState(GameState.Playing);
        ResetHidingSpots();
    }
    public void PauseGame()
    {
        ChangeState(GameState.Paused);
        isPaused = true;
    }
    public void ResumeGame()
    {
        ChangeState(GameState.Resumed);
        isPaused = false;
    }
    public void WinGame()
    {
        OnGameWin.Invoke();
        ChangeState(GameState.Won);
    }
    public void LoseGame()
    {
        OnGameOver.Invoke();
        ChangeState(GameState.Lost);
    }
    public void CheckState()
    {
        if (!isAlive)
        {
            LoseGame();
        }
    }
    private void ResetHidingSpots()
    {
        foreach (var spot in hidingSpots)
        {
            spot.isTaken = false;
        }
    }
    public HidingSpot GetHidingSpot()
    {
        Dictionary<HidingSpot, float> dict = new Dictionary<HidingSpot, float>();

        foreach (HidingSpot spot in hidingSpots)
        {
            if (spot.isTaken)
                continue;
            dict.Add(spot, spot.chance);
        }

        HidingSpot selectedSpot = MyRandom.RouletteWheelSelection(dict);
       
        if (selectedSpot != null)
        {
            selectedSpot.isTaken = true;
        }

        return selectedSpot;
    }
    public MyPath GetPath()
    {
        Dictionary<MyPath, float> dict = new Dictionary<MyPath, float>();

        foreach (MyPath path in paths)
        {
            float chance = path.chance;
            dict.Add(path, chance);
        }

        return MyRandom.RouletteWheelSelection(dict);
    }

    // la funcion la llame Add pq al restar puntos el puesto en el que termina el player aumenta
    public void AddPoints()
    {
        if (points <= 1)
        {
            WinGame();
        }
        else 
        {
            points--;
        }
    }
}