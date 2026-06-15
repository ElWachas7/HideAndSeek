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
    private void ClearEvents()
    {
        GameManager.Instance.OnGameOver -= OnLose;
        GameManager.Instance.OnGameWin -= OnWin;
    }


    public void OnStart()
    {
        ClearUI();
        GameManager.Instance.ChangeState(GameState.Playing);
        inGameUI.gameObject.SetActive(true);
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
        ClearUI();
        scoreUI.text = GameManager.Instance.Points.ToString();
        loseUI.gameObject.SetActive(true);
    }

    public void UpdateStamina(float stamina)
    {
        slider.value = 1f - stamina / 10f;
    }
  
}