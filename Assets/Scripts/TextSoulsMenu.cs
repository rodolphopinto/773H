using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSoulsMenu : MonoBehaviour {
    Text text;
	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();
        text.text = GameManager.instance.souls.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = GameManager.instance.souls.ToString();
    }
}
