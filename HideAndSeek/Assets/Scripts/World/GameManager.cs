using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<MyPath> paths = new List<MyPath>();
    public static GameManager Instance;
    public enum GameState { Menu, Playing, Paused, Won, Lost, Resumed }
    [SerializeField] private GameState currentState;
    public bool IsAlive => isAlive;
    private bool isAlive;
    public event Action OnGameOver;
    public event Action OnGameWin;

    public List<HidingEnemy> hidingEnemies;
    public List<HidingSpot> hidingSpots;

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
        OnGameWin.Invoke();
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
            Debug.Log("Spot elegido: " + selectedSpot.Transform.name);
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
}