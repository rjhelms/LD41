using UnityEngine;
using System.Collections;

public class Captain : ProjectileEnemy
{
    public float TargetXPos;

    private bool MoveUp;

    protected override void Start()
    {
        base.Start();
        TargetXPos = transform.position.x;
        MoveUp = false;
        if (Random.value < 0.5f)
            MoveUp = true;
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;
        if (State == AnimState.HIT)
        {
            Facing = -hitDirection;
            moveVector = new Vector3(HitStaggerSpeed, 0, 0) * hitDirection;
        }

        if (State == AnimState.IDLE | State == AnimState.WALK)
        {
            if (transform.position.y >= UpperBound)
            {
                MoveUp = false;
            }
            else if (transform.position.y <= LowerBound)
            {
                MoveUp = true;
            }

            if (MoveUp)
            {
                moveVector += new Vector3(0, WalkSpeed, 0);
            }
            else
            {
                moveVector -= new Vector3(0, WalkSpeed, 0);
            }

            if (PlayerTransform.position.x < transform.position.x)
            {
                Facing = -1;
            }
            else if (PlayerTransform.position.x > transform.position.x)
            {
                Facing = 1;

            }

            // check if we're at target x-pos
            if (transform.position.x != TargetXPos)
            {
                // if no, move towards x-pos diagonally
                if (transform.position.x > TargetXPos)
                {
                    moveVector -= new Vector3(WalkSpeed, 0, 0);
                    Facing = -1;
                }
                else
                {
                    moveVector = new Vector3(WalkSpeed, 0, 0);
                    Facing = 1;
                }
            }
            ChangeAnimState(AnimState.WALK);

            if (Random.value <= FireChance)
            {
                DoAttack();
            }
        }

        transform.position += moveVector;
    }


}
