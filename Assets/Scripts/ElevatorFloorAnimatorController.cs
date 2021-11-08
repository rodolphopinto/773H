using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloorAnimatorController : MonoBehaviour {

    public Sprite[] spriteElevator;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;

    private SpriteRenderer spriteRender;

	void Start () {
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        SpriteController();
    }
    
    void SpriteController()
    {
        float posY = transform.position.y;

        if (posY > 6.4f)
            spriteRender.sprite = spriteElevator[0];
        else if(posY < 5.9f && posY > 5f)
            spriteRender.sprite = spriteElevator[1];
        else if (posY < 4.5f && posY > 3.7f)
            spriteRender.sprite = spriteElevator[2];
        else if (posY < 3.2f && posY > 2.3f)
            spriteRender.sprite = spriteElevator[3];
        else if (posY < 1.9f && posY > 1f)
            spriteRender.sprite = spriteElevator[4];
        else if (posY < 0.5f && posY > -0.5f)
            spriteRender.sprite = spriteElevator[5];
        else if (posY < -0.8f && posY > -1.7f)
            spriteRender.sprite = spriteElevator[6];
        else if (posY < -2.2f)
            spriteRender.sprite = spriteElevator[7];
    }

}
