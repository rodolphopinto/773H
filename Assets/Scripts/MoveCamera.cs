using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public float time = 1f;

    private GameObject player;
    private Vector3 finalPosition;
    private Vector3 previousPosition;
    private float speed;
    private Rigidbody2D rb2D;
    private Camera cam;

    void Awake() {
        

        speed = 100f;
        player = GameObject.Find("Player");
        rb2D = GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
        finalPosition = new Vector3(1.7f, 5.25f, -10);
        previousPosition = finalPosition;
        //StartCoroutine(Teste());
        //GameManager.instance.InitGame();
    }
	
	void Update () {
        //Time.timeScale = time;

        //Debug.Log(Time.unscaledDeltaTime + " : " + Time.deltaTime);
        
        if (player != null)
        {
            float size = cam.orthographicSize;
            previousPosition = finalPosition;
            finalPosition = new Vector3(transform.position.x, player.transform.position.y, -10f);
            if (finalPosition.y + size > 8.3f || finalPosition.y - size < -4.1f)
                finalPosition.y = previousPosition.y;
            StartCoroutine(Move());
        }
        GameManager.instance.QueueManager();
        SoundManager.instance.RemoveAudioEffectPerson();
	}

    private IEnumerator Move()
    {
        float sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, (Vector3)finalPosition, speed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - (Vector3)finalPosition).sqrMagnitude;
            yield return null;
        }
    }

    private IEnumerator Teste()
    {
        Debug.Log("foi1");
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 0;
        Debug.Log("foi2");
        yield return new WaitForSecondsRealtime(2f);
        Debug.Log("foi3");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(2f);
        Debug.Log("foi4");
    }
}
