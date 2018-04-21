using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor {

    public Collider2D PunchCollider;
    public Collider2D JumpCollider;

    // Update is called once per frame
    protected override void Update () {

        // punch
        if (Input.GetButtonDown("Fire1"))
        {
            if (AnimState < 2)
            {
                DoAttack();
            }
        }

        // jump
        if (Input.GetButtonDown("Fire2"))
        {
            if (AnimState < 3)
            {
                DoJump();
            }
        }
        base.Update();

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
        ChangeAnimState(2);
    }

    private void DoJump()
    {
        ChangeAnimState(3);
        JumpCurrentSpeed = JumpStartSpeed;
        JumpAccelFrame = false;
        IsJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == PunchCollider | collision.otherCollider == JumpCollider)
        {
            // we've hit something!
            Debug.Log(collision.gameObject);
            collision.gameObject.GetComponent<BaseActor>().Hit();
        }
    }
}
