using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

    public float speed = 1f;
    public GameManager.Floors sins;
    public int occupiedSpaceSize = 1;
    public Sprite[] hairs;
    public Sprite[] clothes;
    public string[] descriptions;
    public AudioClip[] audioEffects;
    public AudioClip audioSleepEffect;

    [HideInInspector] public GameManager.Floors floor;
    [HideInInspector] public bool insideElevator;
    [HideInInspector] public Vector2 finalPosition = new Vector2(-1.2f, 6.83f);
    [HideInInspector] public Sprite sprite;
    [HideInInspector] public string description;

    private Rigidbody2D rb2D;
    private Animator animator;
    private Vector2 positionOrigin = -Vector2.one;
    private bool movingPerson;
    private bool sleeping;
    private int timeSleep;
    private AudioSource audioSourceEffect;
    private int timeAudioEffect;


    void Start () {
        GameObject hair = new GameObject("Hear");
        SpriteRenderer spriteRenderHair = hair.AddComponent<SpriteRenderer>();
        int rand = Random.Range(0, hairs.Length);
        if (rand >= hairs.Length)
            rand = hairs.Length - 1;
        spriteRenderHair.sprite = hairs[rand];
        spriteRenderHair.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        spriteRenderHair.sortingLayerName = "Person";
        spriteRenderHair.sortingOrder = 1;
        Instantiate(hair, gameObject.transform);
        Destroy(hair);

        GameObject clothe = new GameObject("Clothes");
        SpriteRenderer spriteRenderClothe = clothe.AddComponent<SpriteRenderer>();
        rand = Random.Range(0, clothes.Length);
        if (rand >= clothes.Length)
            rand = clothes.Length - 1;
        spriteRenderClothe.sprite = clothes[rand];
        spriteRenderClothe.color = gameObject.GetComponent<SpriteRenderer>().color;
        spriteRenderClothe.sortingLayerName = "Person";
        spriteRenderClothe.sortingOrder = 1;
        Instantiate(clothe, gameObject.transform);
        Destroy(clothe);

        audioSourceEffect = GetComponent<AudioSource>();

        transform.position = new Vector2(4.5f, 6.83f);
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        positionOrigin = transform.position;
        sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        rand = Random.Range(0, descriptions.Length);
        if (rand >= descriptions.Length)
            rand = description.Length - 1;
        description = descriptions[rand];

        movingPerson = true;
        insideElevator = false;
        sleeping = false;
        timeSleep = (int)(Time.time + Random.Range(3f, 7f));
        timeAudioEffect = (int)(Time.time + Random.Range(2f, 9f));

        GameManager.instance.persons.Add(gameObject);
    }
	
	void Update () {
        if (sins == GameManager.Floors.Sloth)
            CharacteristicSloth();

        if (!insideElevator)
            StartCoroutine(Move());

        AudioController();
        //Debug.Log(finalPosition);
    }

    private IEnumerator Move()
    {
        Moving();
        float sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            if(sleeping)
                finalPosition = transform.position;
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, (Vector3)finalPosition, speed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;
            if((Vector2)transform.position == finalPosition)
                positionOrigin = transform.position;
            if (transform.position.x > 6f && floor != GameManager.Floors.WaitingRoom)
                Destroy(gameObject);
            yield return null;
        }
    }

    private void Moving()
    {
        if (positionOrigin.x == transform.position.x)
        {
            if (movingPerson)
            {
                animator.SetBool("MovingPerson", false);
                movingPerson = false;
            }
        }
        else
        {
            if (finalPosition.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else if (finalPosition.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);

            if (!movingPerson)
            {
                animator.SetBool("MovingPerson", true);
                movingPerson = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && floor == GameManager.Floors.WaitingRoom)
        {
            animator.SetBool("MovingPerson", false);
            movingPerson = false;
            positionOrigin = transform.position;
        }
    }

    public IEnumerator CharacteristicGreed()
    {
        yield return new WaitForSeconds(10f);
        if (insideElevator)
            GameManager.instance.currentSouls /= 2;
    }

    public void CharacteristicSloth()
    {
        if(transform.position.x < 3f && transform.position.x > 1.165f && floor == GameManager.Floors.WaitingRoom
            && !sleeping && timeSleep <= (int)Time.time)
        {
            int rand = Random.Range(0, 10);;
            if (rand <= 1)
            {
                audioSourceEffect.Stop();
                audioSourceEffect.loop = true;
                audioSourceEffect.clip = audioSleepEffect;
                audioSourceEffect.pitch = Random.Range(0.95f, 1.05f);
                audioSourceEffect.Play();

                sleeping = true;
                Invoke("WakeUp", Random.Range(4f, 6f));
            }
            timeSleep = (int)(Time.time + Random.Range(1f, 2f));
        }
    }

    private void WakeUp()
    {
        audioSourceEffect.Stop();
        audioSourceEffect.loop = false;

        sleeping = false;
        finalPosition = new Vector2(1.165f, 6.83f);
        timeSleep = (int)(Time.time + Random.Range(5f, 7f));
    }

    private void AudioController()
    {
        if (timeAudioEffect <= (int)Time.time && transform.position.x < 3.4f && !sleeping && !insideElevator)
        {
            int rand = Random.Range(0, audioEffects.Length);
            if (rand >= audioEffects.Length)
                rand = audioEffects.Length - 1;
            audioSourceEffect.clip = audioEffects[rand];
            timeAudioEffect = (int)(Time.time + Random.Range(4f, 9f));

            if (!sleeping)
                SoundManager.instance.SetAudioEffectPerson(audioSourceEffect);
        }
    }
}
