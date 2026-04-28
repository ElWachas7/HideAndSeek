using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // meter los paneles

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Slider slider;
    private float stamina = 10;
    void Start()
    {
        OnMainMenu();
        GameManager.Instance.OnGameOver += OnLose;
        GameManager.Instance.OnGameWin += OnWin;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWin();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnLose();
        }

        if (Input.GetKey(KeyCode.F))
        {
            stamina -= Time.deltaTime;
            if(stamina <= 0)
            {
                stamina = 10;
            }
            slider.value = 1f - stamina / 10f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnTryPause();
        }
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
        loseUI.gameObject.SetActive(true);
    }
}