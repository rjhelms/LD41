using UnityEngine;
using System.Collections;

public class Sailor : BaseEnemy
{

    private void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;
        if (State < AnimState.HIT & HangFrame == false)
        {
            moveVector = TrackPlayer() * WalkSpeed;
            if (moveVector.magnitude != 0)
            {
                ChangeAnimState(AnimState.WALK);
                if (moveVector.x < 0)
                {
                    Facing = -1;
                }
                else if (moveVector.x > 0)
                {
                    Facing = 1;
                }
            }
        }
        
        if (State == AnimState.HIT)
        {
            Facing = -hitDirection;
            moveVector = new Vector3(HitStaggerSpeed, 0, 0) * hitDirection;
        }

        transform.position += moveVector;
        HangFrame = false;
    }

    private Vector3 TrackPlayer()
    {
        int xTrack = 0;
        int yTrack = 0;
        if (PlayerTransform.position.x < transform.position.x)
            xTrack = -1;
        if (PlayerTransform.position.x > transform.position.x)
            xTrack = 1;

        if (Random.value >= TrackChance)
            xTrack = 0;

        // track player Z, to follow the base of the jump
        if (PlayerTransform.position.z < transform.position.y)
            yTrack = -1;
        if (PlayerTransform.position.z > transform.position.y)
            yTrack = 1;

        if (Random.value >= TrackChance)
            yTrack = 0;

        return new Vector3(xTrack, yTrack, 0);
    }

    public override void Hit(int direction)
    {
        Debug.Log("Ouch!");
        HitPoints--;
        ChangeAnimState(AnimState.HIT);
        hitDirection = direction;
        hitStaggerEnd = Time.time + HitStaggerTime;
        rigidbody2D.simulated = false;
    }

    protected override void EndHit()
    {
        ChangeAnimState(AnimState.IDLE);
        if (HitPoints > 0)
        {
            rigidbody2D.simulated = true;
        } else
        {
            ChangeAnimState(AnimState.DEAD);
            deadStaggerEnd = Time.time + DeadStaggerTime;
            deadFlashNext = Time.time + DeadFlashTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            bool otherHangFrame = collision.gameObject.GetComponent<BaseEnemy>().HangFrame;

            if (!otherHangFrame & Random.value < HangFrameChance)
            {
                HangFrame = true;
                Debug.Log("Hanging back!");
            }
        }
    }
}
