using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLinks : MonoBehaviour
{
    public void OpenWebPage(string url)
    {
        Application.OpenURL(url);
    }
}
