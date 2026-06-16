using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MusicManager>();
                if (_instance == null) 
                {
                    GameObject MusicSingleton = new GameObject("MusicManager_Singleton");
                    _instance = MusicSingleton.AddComponent<MusicManager>();
                    DontDestroyOnLoad(MusicSingleton);
                }
            }
            return _instance;
        }
    }
}
