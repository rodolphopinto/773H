using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public GameObject[] typePersons;

    public int createPersonTimeRandMin;
    public int createPersonTimeRandMax;
    public int bounty;

    public AudioClip menuSound;
    public AudioClip gameSound;

    [HideInInspector] public Text textQueue;
    [HideInInspector] public Text textSouls;
    [HideInInspector] public Text textChance;
    [HideInInspector] public Button finishDay;
    [HideInInspector] public GameObject gameOverScreen;
    [HideInInspector] public Text harvestedSoulsText;
    [HideInInspector] public Text soulsGameOverText;

    [HideInInspector] public int chance;
    [HideInInspector] public int queueSize;
    [HideInInspector] public int maximumQueueSize;
    [HideInInspector] public int souls;
    [HideInInspector] public int currentSouls;
    [HideInInspector] public int demand;
    [HideInInspector] public bool doorIsClossed;
    [HideInInspector] public Floors elevatorFloors;
    [HideInInspector] public List<GameObject> persons;
    [HideInInspector] public int createPersonTime;
    [HideInInspector] public bool firstTime;
    [HideInInspector] public bool canClose;
    [HideInInspector] public bool isCatastropheMode;
    [HideInInspector] public bool isGameOver;


    public enum Sins { Lust, Gluttony, Greed, Wrath, Sloth, Envy, Pride }

    public enum Floors { WaitingRoom, Lust, Gluttony, Greed, Wrath, Sloth, Envy, Pride }

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void InitGame()
    {
        isGameOver = false;
        persons = new List<GameObject>();
        //typePersons[0]
        //persons.Add(typePersons[0].gameObject);
        //typePersons[0].GetComponent<Person>().finalPosition = new Vector2(1.1f, 6.5f);
        //Instantiate(typePersons[0]);
        gameOverScreen = GameObject.Find("Canvas/GameOverScreen");
        harvestedSoulsText = GameObject.Find("Canvas/GameOverScreen/HarvestedSouls").GetComponent<Text>();
        soulsGameOverText = GameObject.Find("Canvas/GameOverScreen/Souls").GetComponent<Text>();
        gameOverScreen.SetActive(false);

        textQueue = GameObject.Find("Canvas/TopGUI/Queue/TextQueue").GetComponent<Text>();
        textSouls = GameObject.Find("Canvas/TopGUI/Souls/TextSouls").GetComponent<Text>();
        textChance = GameObject.Find("Canvas/TopGUI/Chance/TextChance").GetComponent<Text>();
        finishDay = GameObject.Find("Canvas/TopGUI/ButtonFinishDay").GetComponent<Button>();
        finishDay.onClick.AddListener(TaskOnClick);
        chance = 3;
        queueSize = 0;
        maximumQueueSize = 50;
        //souls = 0;
        currentSouls = 0;
        demand = 1;
        createPersonTime = (int)Time.time + 1;

        doorIsClossed = true;
        elevatorFloors = Floors.WaitingRoom;
        firstTime = false;
        canClose = true;
        Time.timeScale = 1f;
        SoundManager.instance.PlayerMusic(gameSound);
    }

    public void InitMenu()
    {
        isCatastropheMode = false;
        SoundManager.instance.PlayerMusic(menuSound);
    }

    void Update()
    {

    }

    public void QueueManager()
    {
        if (createPersonTime <= (int)Time.time && queueSize <= maximumQueueSize)
        {
            //Debug.Log(createPersonTime);
            //persons.Add(typePersons[0]);
            int rand = Random.Range(0, typePersons.Length);
            if (rand >= typePersons.Length)
                rand = typePersons.Length - 1;
            GameObject person = typePersons[rand];
            Instantiate(person);
            createPersonTime = (int)Time.time + Random.Range(createPersonTimeRandMin, createPersonTimeRandMax);
            queueSize++;
        }

        GameObject previousPerson = null;

        foreach(GameObject person in persons)
        {
            
            if (previousPerson == null)
            {
                person.GetComponent<Person>().finalPosition = new Vector2(1.165f, 6.83f);
            }
            else
            {
                Person previousPersonComponent = previousPerson.GetComponent<Person>();
                person.GetComponent<Person>().finalPosition = (Vector2)previousPersonComponent.transform.position + new Vector2(0.4f, 0);
            }

            previousPerson = person;
        }

        textQueue.text = "" + queueSize + "/" + maximumQueueSize;
        textSouls.text = currentSouls.ToString();
        textChance.text = chance.ToString();
        if (queueSize > maximumQueueSize || chance <= 0)
            GameOver(true);
    }

    public void GameOver(bool lostSouls)
    {
        if (!isGameOver)
        {
            isGameOver = true;
            SoundManager.instance.StopMusic();
            if (lostSouls)
                currentSouls /= 2;
            int currentSoulsTemp = currentSouls;
            int soulsTemp = souls;
            souls += currentSouls;
            Save();
            StartCoroutine(GameOverScreen(currentSoulsTemp, soulsTemp));
        }
    }

    private IEnumerator GameOverScreen(float currentSoulsTemp, float soulsTemp)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(1.5f);
        gameOverScreen.SetActive(true);
        harvestedSoulsText.text = ((int)currentSoulsTemp).ToString();
        soulsGameOverText.text = ((int)soulsTemp).ToString();
        yield return new WaitForSecondsRealtime(1f);
        float speedTime = currentSoulsTemp / 2f;
        while (currentSoulsTemp > 0)
        {
            currentSoulsTemp -= speedTime * Time.unscaledDeltaTime;
            soulsTemp += speedTime * Time.unscaledDeltaTime;
            if (currentSoulsTemp <= 0)
            {
                currentSoulsTemp = 0;
                soulsTemp = souls;
            }
            harvestedSoulsText.text = ((int)currentSoulsTemp).ToString();
            soulsGameOverText.text = ((int)soulsTemp).ToString();
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(3f);
        StopAllCoroutines();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void AddCurrentSouls(int souls)
    {
        currentSouls = souls;
    }

    private void TaskOnClick()
    {
        GameOver(false);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/773HSave.ssth");

        SaveGame saveGame = new SaveGame();
        saveGame.souls = souls;

        bf.Serialize(file, saveGame);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/773HSave.ssth"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/773HSave.ssth", FileMode.Open);
            SaveGame saveGame = (SaveGame)bf.Deserialize(file);
            file.Close();

            souls = saveGame.souls;
            firstTime = false;
        }
        else
        {
            souls = 0;
            firstTime = true;
        }
    }
}

[System.Serializable]
public class SaveGame
{
    public int souls;
}
