using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // meter los paneles
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI scoreAlly;

    public static UIManager Instance;

    public void UpdateUI() => UpdateUi();
    public void Awake()
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
    void Start()
    {
        GameManager.Instance.OnGameOver += OnLose;
        GameManager.Instance.OnGameWin += OnWin;
        OnStart();
    }

    private void ClearUI()
    {
        pauseUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
        loseUI.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(false);
    }
    public void OnStart()
    {
        ClearUI();
        GameManager.Instance.ChangeState(GameState.Playing);
        inGameUI.gameObject.SetActive(true);
    }
    private void UpdateUi() 
    {
        scoreUI.text = GameManager.Instance.CurrentCoins.ToString();
        scoreAlly.text = GameManager.Instance.AlliesAlive.ToString();
    }

    public void OnTryPause()
    {
        if(GameManager.Instance.CurrentState == GameState.Menu)
        {
            return;
        }
        ClearUI();
        if(GameManager.Instance.IsPaused == false)
        {
            GameManager.Instance.PauseGame();
            pauseUI.gameObject.SetActive(true);

        } else if (GameManager.Instance.IsPaused == true)
        {
            GameManager.Instance.ResumeGame();
            inGameUI.gameObject.SetActive(true);
        } 
    }
    public void OnMainMenu()
    {
        ClearUI();
        GameManager.Instance.ChangeState(GameState.Menu);
        SceneManager.LoadScene("MainMenu");
    }
    public void OnWin()
    {
        ClearUI();
        winUI.gameObject.SetActive(true);
    }

    public void OnLose()
    {
        scoreUI.text = GameManager.Instance.CurrentCoins.ToString();
        loseUI.gameObject.SetActive(true);
    }

    public void UpdateStamina(float stamina)
    {
        slider.value = stamina / 10f;
    }
}