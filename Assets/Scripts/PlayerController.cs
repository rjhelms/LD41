using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Sprite IdleSprite;
    public Sprite[] WalkSprites;
    public Sprite[] PunchSprites;
    public Sprite JumpSprite;
    public float WalkSpriteTime;
    public float PunchSpriteTime = 1.0f;

    public int Facing = 1;
    public float WalkSpeed = 1.0f;
    public int AnimState = 0;
    public int AnimSpriteCount = 0;
    public float AnimNextSpriteTime = 0;
    public float PunchEndTime = 0;

    public int LeftBound = 24;
    public int RightBound = 296;
    public int UpperBound = 96;
    public int LowerBound = 8;

    public bool IsJumping = false;
    public float JumpStartSpeed = 10.0f;
    public float JumpCurrentSpeed = 10.0f;
    public bool JumpAccelFrame = false;
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
            case 0: // idle
                spriteRenderer.sprite = IdleSprite;
                break;
            case 1: // walk
                if (Time.time > AnimNextSpriteTime)
                {
                    AnimSpriteCount++;
                    if (AnimSpriteCount >= WalkSprites.Length)
                        AnimSpriteCount = 0;
                    AnimNextSpriteTime += WalkSpriteTime;
                }
                spriteRenderer.sprite = WalkSprites[AnimSpriteCount];
                break;
            case 2: // punch
                if (Facing == 1)
                {
                    spriteRenderer.sprite = PunchSprites[1];
                } else
                {
                    spriteRenderer.sprite = PunchSprites[0];
                }
                break;
            case 3: // jump
                spriteRenderer.sprite = JumpSprite;
                break;
        }
	}

    private void FixedUpdate()
    {
        int newState = 0;
        Vector3 moveVector = new Vector3(0, 0, 0);
        if (Input.GetAxis("Horizontal") > 0)
        {
            if (transform.position.x < RightBound)
                moveVector += new Vector3(1, 0, 0) * WalkSpeed;
            newState = 1;
            Facing = 1;
        } else if (Input.GetAxis("Horizontal") < 0)
        {
            if (transform.position.x > LeftBound)
                moveVector -= new Vector3(1, 0, 0) * WalkSpeed;
            else CheckRightBound();
            newState = 1;
            Facing = -1;
        }

        // punch
        if (Input.GetButtonDown("Fire1"))
        {
            if (AnimState < 2)
            {
                newState = 2;
                DoPunch();
            }
        }

        // process jump frame
        if (IsJumping)
        {
            moveVector += new Vector3(0, JumpCurrentSpeed, 0);
            if (JumpAccelFrame)
                JumpCurrentSpeed -= 1;
            if (JumpCurrentSpeed < -JumpStartSpeed)
                IsJumping = false;
            JumpAccelFrame = !JumpAccelFrame;
        }
        // jump
        if (Input.GetButtonDown("Fire2"))
        {
            if (AnimState < 3)
            {
                DoJump();
            }
        }

        if (AnimState != 3) // can't move vert during jump
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                if (transform.position.y < UpperBound)
                {
                    moveVector += new Vector3(0, 1, 0) * WalkSpeed;
                    newState = 1;
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                if (transform.position.y > LowerBound)
                {
                    moveVector -= new Vector3(0, 1, 0) * WalkSpeed;
                    newState = 1;
                }
            }
        }

        ChangeAnimState(newState);
        transform.position += moveVector;
    }

    private void ChangeAnimState(int newState)
    {
        // don't process any logic if this isn't actually a state change
        if (AnimState == newState)
        {
            return;
        }

        // don't change states during jump!
        if (IsJumping)
        {
            return;
        }

        // don't end punch state early for walk or idle
        if (AnimState == 2 & newState < 2 & Time.time < PunchEndTime)
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

        // walk
        if (newState == 1)
        {
            AnimSpriteCount = 1;
            AnimNextSpriteTime = Time.time + WalkSpriteTime;
            AnimState = 1;
            return;
        }

        // punch
        if (newState == 2)
        {
            AnimSpriteCount = 0;
            AnimNextSpriteTime = 0;
            PunchEndTime = Time.time + PunchSpriteTime;
            AnimState = 2;
            return;
        }

        if (newState == 3)
        {
            AnimSpriteCount = 0;
            AnimNextSpriteTime = 0;
            AnimState = 3;
            return;
        }
    }

    private void CheckRightBound()
    {
        // process logic for when a player is trying to walk off the right screen bound

    }
    
    private void DoPunch()
    {
        // logic for punching goes here
    }

    private void DoJump()
    {
        ChangeAnimState(3);
        JumpCurrentSpeed = JumpStartSpeed;
        JumpAccelFrame = false;
        IsJumping = true;
    }
}
