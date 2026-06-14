using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redirectNumber;
    public void StartTransition()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            if (redirectNumber != null)
            {
                redirectNumber.text = $"Redirecting {i}";
            }
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("Map1");
    }
}
