using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Sprite IdleSprite;
    public Sprite[] WalkSprites;
    public float WalkSpriteTime;

    public int Facing = 1;
    public float WalkSpeed = 1.0f;
    public int AnimState = 0;
    public int AnimSpriteCount = 0;
    public float AnimNextSpriteTime = 0;

    public int LeftBound = 24;
    public int RightBound = 296;
    public int UpperBound = 96;
    public int LowerBound = 8;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(Facing, 1, 0);
        switch (AnimState)
        {
            case 0:
                spriteRenderer.sprite = IdleSprite;
                break;
            case 1:
                if (Time.time > AnimNextSpriteTime)
                {
                    AnimSpriteCount++;
                    if (AnimSpriteCount >= WalkSprites.Length)
                        AnimSpriteCount = 0;
                    AnimNextSpriteTime += WalkSpriteTime;
                }
                spriteRenderer.sprite = WalkSprites[AnimSpriteCount];
                break;
        }
	}

    private void FixedUpdate()
    {
        Vector3 moveVector = new Vector3(0, 0, 0);
        if (Input.GetAxis("Horizontal") > 0)
        {
            if (transform.position.x < RightBound)
                moveVector += new Vector3(1, 0, 0) * WalkSpeed;
            Facing = 1;
        } else if (Input.GetAxis("Horizontal") < 0)
        {
            if (transform.position.x > LeftBound)
                moveVector -= new Vector3(1, 0, 0) * WalkSpeed;
            else CheckRightBound();
            Facing = -1;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            if (transform.position.y < UpperBound)
                moveVector += new Vector3(0, 1, 0) * WalkSpeed;
        } else if (Input.GetAxis("Vertical") < 0)
        {
            if (transform.position.y > LowerBound)
                moveVector -= new Vector3(0, 1, 0) * WalkSpeed;
        }

        // gotta be a better way to check for idle state
        if (moveVector.magnitude < 1)
        {
            ChangeAnimState(0);
        } else
        {
            ChangeAnimState(1);
        }

        transform.position += moveVector;
    }

    private void ChangeAnimState(int newState)
    {
        // don't process any logic if this isn't actually a state change
        if (AnimState == newState)
        {
            return;
        }

        // idle
        if (newState == 0)
        {
            AnimSpriteCount = 0;
            AnimNextSpriteTime = 0;
            AnimState = 0;
            return;
        }

        // walking
        if (newState == 1)
        {
            AnimSpriteCount = 1;
            AnimNextSpriteTime = Time.time + WalkSpriteTime;
            AnimState = 1;
            return;
        }
    }

    private void CheckRightBound()
    {
        // process logic for when a player is trying to walk off the right screen bound

    }
}
