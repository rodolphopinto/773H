using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonCatastropheMode : MonoBehaviour {

    public AudioClip soundCatastropheMode;

    private Button btn;

    void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {
        SoundManager.instance.StopMusic();
        GameManager.instance.isCatastropheMode = true;
        GameManager.instance.createPersonTimeRandMin = 1;
        GameManager.instance.createPersonTimeRandMax = 4;
        GameManager.instance.bounty = 3;
        GameManager.instance.gameSound = soundCatastropheMode;
        SceneManager.LoadScene("Game");
    }
}
