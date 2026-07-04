using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private GameState currentState;
    [SerializeField] private Dictionary<MyNode, float> _hidingSpots = new Dictionary<MyNode, float>();
    [SerializeField] private Dictionary<MyNode, float> _searchingSpots = new Dictionary<MyNode, float>();
    [SerializeField] private Dictionary<Vector3, float> _points = new Dictionary<Vector3, float>();
    #endregion

    #region Variables
    private int points = 5;
    private bool isPaused = false;
    private int _currentCoins;
    private int _alliesAlive;
    #endregion

    #region Properties
    public GameState CurrentState => currentState;
    public bool IsPaused => isPaused;
    public int CurrentCoins => _currentCoins;
    public int AlliesAlive => _alliesAlive;
    #endregion

    #region Events/Global
    public static GameManager Instance;
    public event Action OnGameOver;
    public event Action OnGameWin;
    #endregion

    #region MagicMethods
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
    #endregion

    #region GameLoop
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                Debug.Log("Menu");
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
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
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        ChangeState(GameState.Menu);
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
    public void RegisterCoin()
    {
        _currentCoins++;
        UIManager.Instance.UpdateUI();
    }
    public void UnregisterCoin()
    {
        _currentCoins--;
        UIManager.Instance.UpdateUI();
        if (_currentCoins <= 0)
            WinGame();
    }
    public void RegisterEnemy()
    {
        _alliesAlive++;
        UIManager.Instance.UpdateUI();
    }
    public void UnregisterEnemy()
    {
        _alliesAlive--;
        UIManager.Instance.UpdateUI();
    }
    #endregion

    #region PathPoints
    public void AddHidingSpot(MyNode node, float chance)
    {
        if (!_hidingSpots.ContainsKey(node))
        {
            _hidingSpots.Add(node, chance);
        }
    }
    public void AddSearchingSpot(MyNode node, float chance)
    {
        if (!_searchingSpots.ContainsKey(node))
        {
            _searchingSpots.Add(node, chance);
        }
    }
    public void AddPoint(Vector3 Position, float chance) 
    {
        if (!_points.ContainsKey(Position)) 
        {
            _points.Add(Position, chance);
        }
    }
    private void ResetHidingSpots()
    {
        if (_hidingSpots == null || _searchingSpots == null)
            return;

        foreach (var node in _hidingSpots)
        {
            _hidingSpots[node.Key] = node.Key.Chance;
        }
        foreach (var node in _searchingSpots)
        {
            _searchingSpots[node.Key] = node.Key.Chance;
        }
        Debug.Log("Reset Chances Succesfully");
    }
    public void UpdateNodesValues<T>(T node, Dictionary<T, float> dict)
    {
        // esto guarda las chances del nodo, y las reparte 1 por 1 entre los otros nodos
        // el nodo actual queda con 0 chances de aparecer luego
        // pero cuando se van eligiendo otros nodos este va a ir ganando puntito a puntito hasta que vuelva a ser el mas valorado
        float valueToShare = 0;
        if (dict.TryGetValue(node, out float value))
        {
            valueToShare = value;
            dict[node] = 0;
        }

        List<T> keys = new List<T>(dict.Keys);

        while (valueToShare > 0)
        {
            foreach (T key in keys)
            {
                if (valueToShare <= 0) break;
                dict[key] += 1f;
                valueToShare--;
            }
        }
    }
    public MyNode GetHidingSpot()
    {
        MyNode selectedSpot = MyRandom.RouletteWheelSelection(_hidingSpots);
        UpdateNodesValues(selectedSpot, _hidingSpots);
        return selectedSpot;
    }
    public MyNode GetSearchingSpot()
    {
        MyNode selectedSpot = MyRandom.RouletteWheelSelection(_searchingSpots);
        UpdateNodesValues(selectedSpot, _searchingSpots);
        return selectedSpot;
    }
    public Vector3 GetPoint() 
    {
        Vector3 point = MyRandom.RouletteWheelSelection(_points);
        UpdateNodesValues(point, _points);
        return point;
    }
    #endregion
}