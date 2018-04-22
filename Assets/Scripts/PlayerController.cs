using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor {

    public float HitInvulnTime = 0.5f;
    public float HitFlashTime = 0.1f;

    public bool IsInvulnerable;

    public Collider2D PunchCollider;
    public Collider2D JumpCollider;

    private float hitInvulnEnd;
    private float hitNextFlash;

    // Update is called once per frame
    protected override void Update () {

        // punch
        if (Input.GetButtonDown("Fire1"))
        {
            if (State < AnimState.ATTACK)
            {
                DoAttack();
            }
        }

        // jump
        if (Input.GetButtonDown("Fire2"))
        {
            if (State < AnimState.JUMP)
            {
                DoJump();
            }
        }
        base.Update();
        if (IsInvulnerable)
        {
            if (Time.time > hitNextFlash)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                hitNextFlash += HitFlashTime;
            }
            if (Time.time > hitInvulnEnd)
                EndHit();
        }
    }

    private void FixedUpdate()
    {
        AnimState newState = AnimState.IDLE;
        Vector3 moveVector = new Vector3(0, 0, 0);
        if (Input.GetAxis("Horizontal") > 0)
        {
            if (transform.position.x < RightBound)
                moveVector += new Vector3(1, 0, 0) * WalkSpeed;
            newState = AnimState.WALK;
            Facing = 1;
        } else if (Input.GetAxis("Horizontal") < 0)
        {
            if (transform.position.x > LeftBound)
                moveVector -= new Vector3(1, 0, 0) * WalkSpeed;
            else CheckRightBound();
            newState = AnimState.WALK;
            Facing = -1;
        }
        
        // process jump frame
        if (IsJumping)
        {
            moveVector += new Vector3(0, JumpCurrentSpeed, 0);
            if (JumpAccelFrame)
            {
                JumpCurrentSpeed -= 1;
                if (JumpCurrentSpeed < 0)
                    JumpCollider.enabled = true;
            }
            if (JumpCurrentSpeed < -JumpStartSpeed)
            {
                JumpCollider.enabled = false;
                IsJumping = false;
            }
            JumpAccelFrame = !JumpAccelFrame;
        }


        if (State != AnimState.JUMP) // can't move vert during jump
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                if (transform.position.y < UpperBound)
                {
                    moveVector += new Vector3(0, 1, 0) * WalkSpeed;
                    newState = AnimState.WALK;
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                if (transform.position.y > LowerBound)
                {
                    moveVector -= new Vector3(0, 1, 0) * WalkSpeed;
                    newState = AnimState.WALK;
                }
            }
        }

        if (Time.time > AttackEndTime)
        {
            PunchCollider.enabled = false;
        }

        ChangeAnimState(newState);
        transform.position += moveVector;
    }

    private void CheckRightBound()
    {
        // process logic for when a player is trying to walk off the right screen bound

    }
    
    private void DoAttack()
    {
        // logic for punching goes here
        PunchCollider.enabled = true;
        ChangeAnimState(AnimState.ATTACK);
    }

    private void DoJump()
    {
        ChangeAnimState(AnimState.JUMP);
        JumpCurrentSpeed = JumpStartSpeed;
        JumpAccelFrame = false;
        IsJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {
            if (collision.otherCollider == PunchCollider | collision.otherCollider == JumpCollider)
            {
                collision.gameObject.GetComponent<BaseActor>().Hit(Facing);
            }
            else
            {
                Hit(Facing);
            }
        }
    }

    public override void Hit(int direction)
    {
        if (!IsInvulnerable)
        {
            IsInvulnerable = true;
            HitPoints--;
            hitInvulnEnd = Time.time + HitInvulnTime;
            hitNextFlash = Time.time + HitFlashTime;
        }
    }

    protected override void EndHit()
    {
        spriteRenderer.enabled = true;
        IsInvulnerable = false;
    }
}
