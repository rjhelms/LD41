using UnityEngine;
using System.Collections;

public abstract class BaseActor : MonoBehaviour
{
    public Sprite IdleSprite;
    public Sprite[] WalkSprites;
    public Sprite[] AttackSprites;
    public Sprite JumpSprite;
    public float WalkSpriteTime;
    public float PunchSpriteTime = 1.0f;

    public int Facing = 1;
    public float WalkSpeed = 1.0f;
    public int AnimState = 0;
    public int AnimSpriteCount = 0;
    public float AnimNextSpriteTime = 0;
    public float AttackEndTime = 0;

    public int LeftBound = 24;
    public int RightBound = 296;
    public int UpperBound = 96;
    public int LowerBound = 8;

    public bool IsJumping = false;
    public float JumpStartSpeed = 10.0f;
    public float JumpCurrentSpeed = 10.0f;
    public bool JumpAccelFrame = false;

    protected SpriteRenderer spriteRenderer;

    // Use this for initialization
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
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
                    spriteRenderer.sprite = AttackSprites[1];
                } else
                {
                    spriteRenderer.sprite = AttackSprites[0];
                }
                break;
            case 3: // jump
                spriteRenderer.sprite = JumpSprite;
                break;
        }

        if (AnimState != 3)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }

    protected void ChangeAnimState(int newState)
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
        if (AnimState == 2 & newState < 2 & Time.time < AttackEndTime)
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
            AttackEndTime = Time.time + PunchSpriteTime;
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

    public virtual void Hit()
    {

    }
}
