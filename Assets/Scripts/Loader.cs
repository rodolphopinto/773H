using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

    public GameManager gameManager;
    public SoundManager soundManager;

    void Awake()
    {
        if (SoundManager.instance == null)
            Instantiate(soundManager);
        if (GameManager.instance == null)
            Instantiate(gameManager);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            GameManager.instance.InitGame();
            SoundManager.instance.InitGame();
        }
        else if (scene.name == "Menu")
        {
            GameManager.instance.InitMenu();
            SoundManager.instance.InitMenu();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
