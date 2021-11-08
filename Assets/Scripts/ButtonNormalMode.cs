using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonNormalMode : MonoBehaviour {

    public AudioClip soundNormalMode;

    private Button btn;

    void Start () {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {
        SoundManager.instance.StopMusic();
        GameManager.instance.isCatastropheMode = false;
        GameManager.instance.createPersonTimeRandMin = 3;
        GameManager.instance.createPersonTimeRandMax = 6;
        GameManager.instance.bounty = 1;
        GameManager.instance.gameSound = soundNormalMode;
        SceneManager.LoadScene("Game");
    }
}
