using UnityEngine;
using System.Collections;

public class Buttercup : ProjectileEnemy
{
    private bool MoveRight = false;

    private void FixedUpdate()
    {
        if (Active & gameController.State == GameState.RUNNING)
        {
            Vector3 moveVector = Vector3.zero;
            if (State == AnimState.HIT)
            {
                Facing = -hitDirection;
                moveVector = new Vector3(HitStaggerSpeed, 0, 0) * hitDirection;
            }

            if (State == AnimState.IDLE | State == AnimState.WALK)
            {
                if (transform.position.x >= RightBound)
                {
                    MoveRight = false;
                }
                else if (transform.position.x <= LeftBound)
                {
                    MoveRight = true;
                }

                if (MoveRight)
                {
                    Facing = 1;
                    moveVector += new Vector3(WalkSpeed, 0, 0);
                }
                else
                {
                    Facing = -1;
                    moveVector -= new Vector3(WalkSpeed, 0, 0);
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
}