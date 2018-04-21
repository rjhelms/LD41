using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor {

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
                JumpCurrentSpeed -= 1;
            if (JumpCurrentSpeed < -JumpStartSpeed)
                IsJumping = false;
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

    private void CheckRightBound()
    {
        // process logic for when a player is trying to walk off the right screen bound

    }
    
    private void DoAttack()
    {
        // logic for punching goes here
        ChangeAnimState(2);
    }

    private void DoJump()
    {
        ChangeAnimState(3);
        JumpCurrentSpeed = JumpStartSpeed;
        JumpAccelFrame = false;
        IsJumping = true;
    }
}
