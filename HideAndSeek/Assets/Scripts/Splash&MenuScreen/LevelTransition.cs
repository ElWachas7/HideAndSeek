using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public void OnCredits()
    {
        SceneManager.LoadScene("SplashScreen");
        GameManager.Instance.ChangeState(GameState.Menu);
    }
    public void OnButtonStart()
    {
        SceneManager.LoadScene("Map1");
        GameManager.Instance.ChangeState(GameState.Playing);
    }
    public void OnMainMenu()
    {
        GameManager.Instance.ChangeState(GameState.Menu);
        SceneManager.LoadScene("MainMenu");
    }
}
