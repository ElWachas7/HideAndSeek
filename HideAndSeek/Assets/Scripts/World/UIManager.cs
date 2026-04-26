using UnityEngine;

public class UIManager : MonoBehaviour
{
    // meter los paneles

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject inGameUI;

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

    public void OnResume()
    {
        ClearUI();
        GameManager.Instance.ResumeGame();
        inGameUI.gameObject.SetActive(true);
    }

    public void OnPause()
    {
        ClearUI();
        GameManager.Instance.PauseGame();
        pauseUI.gameObject.SetActive(true);
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