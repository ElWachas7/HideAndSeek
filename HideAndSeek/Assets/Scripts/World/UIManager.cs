using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // meter los paneles

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI scoreUI;
    void Start()
    {
     
        GameManager.Instance.OnGameOver += OnLose;
        GameManager.Instance.OnGameWin += OnWin;
    }


    private void ClearUI()
    {
        menuUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
        loseUI.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(false);
    }

    private void ClearEvents()
    {
        GameManager.Instance.OnGameOver -= OnLose;
        GameManager.Instance.OnGameWin -= OnWin;
    }

    public void OnMainMenu()
    {
        ClearEvents();
        ClearUI();
        GameManager.Instance.MainMenu();
        menuUI.gameObject.SetActive(true);
    }

    public void OnPlayButton()
    {
        ClearUI();
        GameManager.Instance.StartGame();
        inGameUI.gameObject.SetActive(true);
    }

    public void OnTryPause()
    {
        if(GameManager.Instance.CurrentState == GameManager.GameState.Menu)
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

    public void OnWin()
    {
        ClearUI();
        winUI.gameObject.SetActive(true);
    }

    public void OnLose()
    {
        ClearUI();
        scoreUI.text = GameManager.Instance.Points.ToString();
        loseUI.gameObject.SetActive(true);
    }

    public void UpdateStamina(float stamina)
    {
        slider.value = 1f - stamina / 10f;
    }
  
}