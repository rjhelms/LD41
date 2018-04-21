using UnityEngine;
using System.Collections;

public enum AnimState
{
    IDLE,
    WALK,
    ATTACK,
    JUMP,
    HIT,
    DEAD,
}

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
    public AnimState State = AnimState.IDLE;
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

    public int HitPoints = 1;


    protected SpriteRenderer spriteRenderer;
    protected new Rigidbody2D rigidbody2D;

    // Use this for initialization
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.localScale = new Vector3(Facing, 1, 0);

        switch (State)
        {
            case AnimState.IDLE: // idle
                spriteRenderer.sprite = IdleSprite;
                break;
            case AnimState.WALK: // walk
                if (Time.time > AnimNextSpriteTime)
                {
                    AnimSpriteCount++;
                    if (AnimSpriteCount >= WalkSprites.Length)
                        AnimSpriteCount = 0;
                    AnimNextSpriteTime += WalkSpriteTime;
                }
                spriteRenderer.sprite = WalkSprites[AnimSpriteCount];
                break;
            case AnimState.ATTACK: // punch
                if (Facing == 1)
                {
                    spriteRenderer.sprite = AttackSprites[1];
                } else
                {
                    spriteRenderer.sprite = AttackSprites[0];
                }
                break;
            case AnimState.JUMP: // jump
                spriteRenderer.sprite = JumpSprite;
                break;
        }

        if (State != AnimState.JUMP)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }

    protected void ChangeAnimState(AnimState newState)
    {
        // don't process any logic if this isn't actually a state change
        if (State == newState)
        {
            return;
        }

        // don't change states during jump!
        if (IsJumping)
        {
            return;
        }

        // don't end punch state early for walk or idle
        if (State == AnimState.ATTACK & newState < AnimState.ATTACK & Time.time < AttackEndTime)
        {
            return;
        }

        switch (newState)
        {
            case AnimState.IDLE:
                AnimSpriteCount = 0;
                AnimNextSpriteTime = 0;
                break;
            case AnimState.WALK:
                AnimSpriteCount = 1;
                AnimNextSpriteTime = Time.time + WalkSpriteTime;
                break;
            case AnimState.ATTACK:
                AnimSpriteCount = 0;
                AnimNextSpriteTime = 0;
                AttackEndTime = Time.time + PunchSpriteTime;
                break;
            case AnimState.JUMP:
                AnimSpriteCount = 0;
                AnimNextSpriteTime = 0;
                break;
        }

        State = newState;

    }

    public virtual void Hit(int direction)
    {

    }

    protected virtual void EndHit()
    {

    }
}
