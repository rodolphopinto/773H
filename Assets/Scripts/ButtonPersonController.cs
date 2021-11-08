using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonPersonController : MonoBehaviour {

    public Text namePerson;
    public Text description;
    public Image imagePerson;

    [HideInInspector] public bool isOccupied;
    [HideInInspector] public GameObject person;

    public int numberButton = 0;
    private Button btn;
    private GameObject player;
    private float time;


    void Start () {
        player = GameObject.Find("Player");
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

	}
	
    public void InitButton(int n)
    {
        numberButton = n;
    }

    public void TaskOnClick()
    {
        if (person != null)
        {
            bool pride = false;
            if (person.GetComponent<Person>().sins == GameManager.Floors.Pride && GameManager.instance.elevatorFloors != GameManager.Floors.Pride)
                pride = true;
            if (!pride)
            {
                player.GetComponent<Player>().ButtonPersonClicked(numberButton, person, gameObject);
                imagePerson.sprite = null;
                imagePerson.color = new Color(1, 1, 1, 0);
                description.text = "";
                namePerson.text = "Empty";
                isOccupied = false;
                if (person.GetComponent<Person>().sins == GameManager.Floors.Lust)
                {
                    GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPerson");
                    time = 0.15f;
                    foreach (GameObject button in buttons)
                    {
                        ButtonPersonController bpc = button.GetComponent<ButtonPersonController>();
                        if (bpc.isOccupied)
                        {
                            GameManager.instance.canClose = false;
                            StartCoroutine(InvokeTaskOnClick(bpc, time));
                            time += 0.15f;
                        }
                    }
                }
                person = null;
            } 
        }
    }

    IEnumerator InvokeTaskOnClick(ButtonPersonController b, float time)
    {
        yield return new WaitForSeconds(time);
        b.RemoveAll();
        if(time+0.01f >= this.time-0.15f)
            GameManager.instance.canClose = true;
    }

    public void RemoveAll()
    {
        player.GetComponent<Player>().ButtonPersonClicked(numberButton, person, gameObject);
        imagePerson.sprite = null;
        imagePerson.color = new Color(1, 1, 1, 0);
        description.text = "";
        namePerson.text = "Empty";
        isOccupied = false;
        person = null;
    }

    public void CharacteristicWrath()
    {
        if (person.GetComponent<Person>().sins != GameManager.Floors.Wrath)
        {
            player.GetComponent<Player>().ButtonPersonClicked(numberButton, person, gameObject);
            imagePerson.sprite = null;
            imagePerson.color = new Color(1, 1, 1, 0);
            description.text = "";
            namePerson.text = "Empty";
            isOccupied = false;
            person = null;
        }
    }
}
