using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public int maximumCapacity = 10;    
    public float elevatorSpeed = 5f;

    public GameObject buttonPerson;
    public Sprite buttonClose;
    public Sprite buttonOpen;

    public Color[] colorSins;

    public AudioClip audioClosingDoor;
    public AudioClip audioOpeningDoor;
    public AudioClip audioMoviment;

    private int chance; 
    private int souls;
    private int numberPersonsInElevator = 0;
    private GameObject scrollViewPerson;
    private GameObject bottomImage;
    private GameObject mask;
    private GameObject content;
    private Text textFloor;
    //private GameObject[] buttonsPerson;
    public GameManager.Floors floor;
    //private GameObject[] personInElevator;
    private GameObject buttonElevator;
    private Text textButtonElevator;
    private Animator animator;
    private Vector2 touchOrigin = -Vector2.one;
    private Vector2 finalPosition = Vector2.up;
    private Rigidbody2D rb2D;
    private float[] floorPoint;
    private bool isMoving = false;
    private bool doorIsClossed;
    private bool clickedButton = false;
    private float sizeButtonY;
    private bool missedAChance;
    private int wrathCount;
    private float timeWrath;
    //private int timeCloseDoor;
    private AudioSource audioSourceEffect;


    void Start () {
        doorIsClossed = GameManager.instance.doorIsClossed;
        souls = 0;
        wrathCount = 0;
        chance = GameManager.instance.chance;
        missedAChance = false;

        audioSourceEffect = GetComponent<AudioSource>();

        scrollViewPerson = GameObject.Find("Canvas/ScrollViewPerson");
        bottomImage = GameObject.Find("Canvas/ScrollViewPerson/BottomImage");
        textFloor = GameObject.Find("Canvas/ScrollViewPerson/TopImage/Text").GetComponent<Text>();

        CreatScrollViewPerson();
        scrollViewPerson.SetActive(false);

        buttonElevator = GameObject.Find("ButtonElevator");
        //textButtonElevator = GameObject.Find("TextButtonElevator").GetComponent<Text>();
        //Button btn = buttonElevator.GetComponent<Button>();

        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        //btn.onClick.AddListener(TaskOnClick);
        finalPosition = transform.position;

        floor = GameManager.instance.elevatorFloors;
        //personInElevator = new GameObject[maximumCapacity];

        floorPoint = new float[8];
        floorPoint[0] = 6.9f;
        floorPoint[1] = 5.55f;
        floorPoint[2] = 4.2f;
        floorPoint[3] = 2.9f;
        floorPoint[4] = 1.5f;
        floorPoint[5] = 0.2f;
        floorPoint[6] = -1.15f;
        floorPoint[7] = -2.5f;
        //floorPoint[8] = -3.85f;

        //timeCloseDoor = (int)Time.deltaTime;

        transform.position = new Vector2(transform.position.x, floorPoint[0]);
        TaskOnClick();
        
    }

    private void CreatScrollViewPerson()
    {
        mask = GameObject.Find("Canvas/ScrollViewPerson/Mask");
        content =  GameObject.Find("Canvas/ScrollViewPerson/Mask/Content");
        float heightButton = 0;

        //buttonsPerson = new GameObject[maximumCapacity];
        for (int i = 0; i < maximumCapacity; i++)
        {
            GameObject butPer = buttonPerson;
            heightButton = butPer.GetComponent<RectTransform>().sizeDelta.y;
            
            if(i > 0)
            {
                //Vector3 previous = buttonsPerson[i-1].GetComponent<RectTransform>().position;
                butPer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -107 - ((heightButton + 5)* i), 0);
            }
            else
            {
                butPer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -107, 0);
            }
            ButtonPersonController comp = butPer.GetComponent<ButtonPersonController>();
            comp.InitButton(i);
            comp.imagePerson.sprite = null;
            comp.imagePerson.color = new Color(1, 1, 1, 0);
            comp.description.text = "";
            comp.namePerson.text = "Empty";
            comp.isOccupied = false;

            Instantiate(butPer, content.transform);
            //buttonsPerson[i] = butPer;
        }
        sizeButtonY = heightButton;
        PositionScrollViewPerson(sizeButtonY);
        //scrollViewPerson
    }

    private void PositionScrollViewPerson(float heightButton)
    {
        int maskHeight = (int)mask.GetComponent<RectTransform>().sizeDelta.y;
        int height = (int)(((heightButton + 20) * maximumCapacity) + 50);
        if (height < maskHeight)
            height = maskHeight;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(840, height);
        content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(height / 2));
    }

    void Update () {

        float horizontal = 0;
        float vertical = 0;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        if (doorIsClossed)
        {
            //ClickElevator();
            MovementWithMouse(ref horizontal, ref vertical);
        }
        else if (!doorIsClossed && floor == GameManager.Floors.WaitingRoom)
        {
            ClickElevator();
        }

#else

        if (doorIsClossed)
        {
            //TouchElevator();
            MovementWithTouch(ref horizontal, ref vertical);
        }
        else if (!doorIsClossed && floor == GameManager.Floors.WaitingRoom)
        {
            TouchElevator();
        }

#endif

        if (transform.position != (Vector3)finalPosition && finalPosition.y <= floorPoint[0]+0.9f && finalPosition.y >= floorPoint[floorPoint.Length - 1]-0.9f)
        {
            isMoving = true;

            if (!audioSourceEffect.isPlaying)
            {
                audioSourceEffect.loop = true;
                audioSourceEffect.clip = audioMoviment;
                audioSourceEffect.Play();
            }

            //buttonElevator.SetActive(false);
        }
        else
        {
            isMoving = false;
            if (audioSourceEffect.isPlaying && audioSourceEffect.clip == audioMoviment)
            {
                audioSourceEffect.Stop();
                audioSourceEffect.loop = false;
            }

            //buttonElevator.SetActive(true);
        }

        //if (vertical != 0)
            StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if (finalPosition.y > floorPoint[0])
            finalPosition.y = floorPoint[0];
        else if (finalPosition.y < floorPoint[floorPoint.Length-1])
            finalPosition.y = floorPoint[floorPoint.Length-1];

        float sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, (Vector3)finalPosition, elevatorSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;              
            if((Vector2)transform.position == finalPosition)
                setFloor();
            if (transform.position.y >= floorPoint[0] && (Vector2)transform.position == finalPosition && doorIsClossed)
                TaskOnClick();
            yield return null;
        }       
    }

    public void TaskOnClick()
    {
        if (doorIsClossed)
        {
            audioSourceEffect.loop = false;
            audioSourceEffect.pitch = Random.Range(0.95f, 1.05f);
            audioSourceEffect.clip = audioOpeningDoor;
            audioSourceEffect.Play();

            animator.SetBool("DoorIsClossed", false);
            buttonElevator.GetComponent<Image>().sprite = buttonClose;
            //textButtonElevator.text = "Close";
            if (floor != GameManager.Floors.WaitingRoom)
            {
                buttonElevator.SetActive(false);
                scrollViewPerson.SetActive(true);
            }
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            missedAChance = true;
            doorIsClossed = false;
        }
        else
        {
            if(wrathCount > 0)
            {
                GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPerson");
                timeWrath = 0.15f;
                foreach (GameObject button in buttons)
                {
                    ButtonPersonController bpc = button.GetComponent<ButtonPersonController>();
                    if (bpc.isOccupied && bpc.person.GetComponent<Person>().sins != GameManager.Floors.Wrath)
                    {
                        GameManager.instance.canClose = false;
                        StartCoroutine(CharacteristicWrath(bpc, timeWrath));
                        timeWrath += 0.15f;
                    }
                }
            }

            if (numberPersonsInElevator > 3 && floor == GameManager.Floors.WaitingRoom)
                CharacteristicEnvy();

            if (GameManager.instance.canClose)
            {

                audioSourceEffect.loop = false;
                audioSourceEffect.pitch = Random.Range(0.95f, 1.05f);
                audioSourceEffect.clip = audioClosingDoor;
                audioSourceEffect.Play();

                animator.SetBool("DoorIsClossed", true);
                buttonElevator.SetActive(true);
                buttonElevator.GetComponent<Image>().sprite = buttonOpen;
                //textButtonElevator.text = "Open";
                PositionScrollViewPerson(sizeButtonY);
                scrollViewPerson.SetActive(false);
                if (missedAChance && floor != GameManager.Floors.WaitingRoom)
                    chance--;
                if (chance < 0)
                    chance = 0;
                GameManager.instance.chance = chance;
                missedAChance = false;
                gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
                doorIsClossed = true;
                //timeCloseDoor = (int)Time.deltaTime + 2;
            }
        }

        GameManager.instance.doorIsClossed = doorIsClossed;
    }

    public IEnumerator CharacteristicWrath(ButtonPersonController b, float time)
    {
        yield return new WaitForSeconds(time);
        b.CharacteristicWrath();
        if (time + 0.01f >= this.timeWrath - 0.15f)
        {
            GameManager.instance.canClose = true;
            TaskOnClick();
        }
    }

    public void CharacteristicEnvy()
    {
        List<GameObject> person = new List<GameObject>();
        List<GameObject> envy = new List<GameObject>();
        scrollViewPerson.SetActive(true);
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPerson");

        foreach (GameObject btn in buttons)
        {
            if (btn.GetComponent<ButtonPersonController>().isOccupied)
            {
                GameObject per = btn.GetComponent<ButtonPersonController>().person;
                if (per.GetComponent<Person>().sins == GameManager.Floors.Envy)
                    envy.Add(btn);
                else
                    person.Add(btn);
            }
        }

        if (envy.Count > 0 && person.Count >= 3)
        {
            foreach(GameObject btn in envy)
            {
                int rand = Random.Range(0, person.Count);
                if(rand >= person.Count)
                    rand = person.Count - 1;
                Color color = person[rand].GetComponent<ButtonPersonController>().imagePerson.color;
                btn.GetComponent<ButtonPersonController>().imagePerson.color = color;
            }
        }
        scrollViewPerson.SetActive(false);
    }

    public void ButtonPersonClicked(int i, GameObject person, GameObject button)
    {
        Person per = person.GetComponent<Person>();

        person.transform.position = new Vector3(1, transform.position.y - 0.08f/*-0.4f*/, 0);
        per.floor = floor;
        per.finalPosition = new Vector3(7f, transform.position.y - 0.08f/*- 0.4f*/, 0);
        person.SetActive(true);
        per.insideElevator = false;
        numberPersonsInElevator -= per.occupiedSpaceSize;
        if (per.sins == GameManager.Floors.Wrath)
            wrathCount--;

        if (per.sins != floor && GameManager.instance.chance == chance)
            chance--;
        missedAChance = false;

        if (souls != GameManager.instance.currentSouls)
            souls = GameManager.instance.currentSouls;

        if (per.sins == floor)
            souls += GameManager.instance.bounty;        
        else
            souls--;
        if (souls < 0)
            souls = 0;
        GameManager.instance.AddCurrentSouls(souls);
    }

    private void setFloor()
    {
        for (int i = 0; i < floorPoint.Length; i++)
        {
            if (floorPoint[i] == transform.position.y)
                floor = (GameManager.Floors)i;
        }
        if (floor != GameManager.Floors.WaitingRoom)
        {
            scrollViewPerson.GetComponent<Image>().color = colorSins[(int)floor - 1];
            bottomImage.GetComponent<Image>().color = colorSins[(int)floor - 1];
        }
        textFloor.text = floor.ToString();
        GameManager.instance.elevatorFloors = floor;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Person")
        {
            int size = other.gameObject.GetComponent<Person>().occupiedSpaceSize;
            if (floor == GameManager.Floors.WaitingRoom && !doorIsClossed && (numberPersonsInElevator + size) <= maximumCapacity)
            {
                other.gameObject.GetComponent<Person>().insideElevator = true;
                
                //personInElevator[numberPersonsInElevator] = other.gameObject;
                GameManager.instance.persons.Remove(other.gameObject);

                scrollViewPerson.SetActive(true);
                GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPerson");
                
                for (int i = 0; i < buttons.Length; i++)
                {
                      

                    GameObject bnt = buttons[i];

                    ButtonPersonController comp = bnt.GetComponent<ButtonPersonController>();
                    if (!comp.isOccupied)
                    {
                        comp.imagePerson.sprite = other.gameObject.GetComponent<Person>().sprite;
                        comp.description.text = other.gameObject.GetComponent<Person>().description;
                        comp.namePerson.text = other.gameObject.GetComponent<Person>().sins.ToString();
                        comp.imagePerson.color = other.gameObject.GetComponent<SpriteRenderer>().color;
                        comp.person = other.gameObject;
                        comp.isOccupied = true;
                        if(other.gameObject.GetComponent<Person>().sins == GameManager.Floors.Greed)
                            StartCoroutine(other.gameObject.GetComponent<Person>().CharacteristicGreed());
                        if (other.gameObject.GetComponent<Person>().sins == GameManager.Floors.Wrath)
                            wrathCount++;
                        break;
                    }
                }
                scrollViewPerson.SetActive(false);
                other.gameObject.SetActive(false);
                numberPersonsInElevator += size;
                GameManager.instance.queueSize--;

                //Debug.Log(numberPersonsInElevator);
            }
        }
    }

    private void SwipeOrientation(Vector2 touchEnd, ref float horizontal, ref float vertical)
    {
        float y = touchEnd.y - touchOrigin.y;
        touchOrigin.x = -1;
        vertical = y > 0 ? 1 : -1;
    }

    private void MovementWithMouse(ref float horizontal, ref float vertical)
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedButton = Input.mousePosition.y <= ((120f * Screen.height) / 1920f);

            touchOrigin = Input.mousePosition;

            //Debug.Log(touchOrigin + " : " + Screen.height + " : " + ((120f * Screen.height) / 1920f) + " : " + clickedButton);
        }
        int limit = (int)touchOrigin.y - (int)Input.mousePosition.y;
        if (limit < 0)
            limit *= -1;
        bool lmt = (limit > (Screen.height * 0.1f));

        if (!ClickElevator() && lmt)
        {
            if (Input.GetMouseButton(0) && touchOrigin != (Vector2)Input.mousePosition && !clickedButton)
            {
                SwipeOrientation(Input.mousePosition, ref horizontal, ref vertical);
                finalPosition = (Vector2)transform.position + new Vector2(horizontal, vertical);
            }

            if (Input.GetMouseButtonUp(0) && touchOrigin != (Vector2)Input.mousePosition && !clickedButton)
            {
                float point = 0;

                SwipeOrientation(Input.mousePosition, ref horizontal, ref vertical);

                bool found = false;
                for (int i = 0; i < floorPoint.Length; i++)
                {
                    if (vertical < 0 && transform.position.y >= floorPoint[i] && !found)
                    {
                        point = floorPoint[i];
                        found = true;
                    }
                    else if (vertical > 0 && transform.position.y <= floorPoint[i] && !found)
                    {
                        point = floorPoint[i];
                        if (transform.position.y >= floorPoint[i + 1])
                            found = true;
                    }
                }

                finalPosition = new Vector2(transform.position.x, point);
            }
        }
    }

    private bool ClickElevator()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool click = rb2D.OverlapPoint(wp) && !isMoving;

        if (click && Input.GetMouseButtonUp(0) )
        {
            TaskOnClick();
        }

        return click;
    }

    private void MovementWithTouch(ref float horizontal, ref float vertical)
    {
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                clickedButton = myTouch.position.y <= ((120f * Screen.height) / 1920f);
                touchOrigin = myTouch.position;  
            }

            int limit = (int)touchOrigin.y - (int)myTouch.position.y;
            if (limit < 0)
                limit *= -1;
            bool lmt = (limit > (Screen.height * 0.1f));

            if (!TouchElevator() && lmt)
            {
                if ((myTouch.phase == TouchPhase.Stationary || myTouch.phase == TouchPhase.Moved) && touchOrigin != myTouch.position && !clickedButton)
                {
                    SwipeOrientation(myTouch.position, ref horizontal, ref vertical);
                    finalPosition = (Vector2)transform.position + new Vector2(horizontal, vertical);

                }

                if (myTouch.phase == TouchPhase.Ended && touchOrigin != myTouch.position && !clickedButton)
                {
                    float point = 0;

                    SwipeOrientation(myTouch.position, ref horizontal, ref vertical);

                    bool found = false;
                    for (int i = 0; i < floorPoint.Length; i++)
                    {
                        if (vertical < 0 && transform.position.y >= floorPoint[i] && !found)
                        {
                            point = floorPoint[i];
                            found = true;
                        }
                        else if (vertical > 0 && transform.position.y <= floorPoint[i] && !found)
                        {
                            point = floorPoint[i];
                            if (transform.position.y >= floorPoint[i + 1])
                                found = true;
                        }
                    }
                    finalPosition = new Vector2(transform.position.x, point);
                }
            }
        }
    }

    private bool TouchElevator()
    {
        bool click = false;

        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            click = rb2D.OverlapPoint(wp) && !isMoving;

            if (click && myTouch.phase == TouchPhase.Ended)
            {
                TaskOnClick();
            }
        }

        return click;
    }
}