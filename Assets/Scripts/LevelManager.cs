using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;
    public static LevelManager Instance { get; } = _instance;


    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlay()
    {

        SceneManager.LoadScene(1);

    }

    public void OnCredits()
    {
        SceneManager.LoadScene(2);
    }

    public void OnMenu()
    {
        SceneManager.LoadScene(0);
    }
}
