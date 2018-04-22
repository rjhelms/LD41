using UnityEngine;
using System.Collections;

public enum AttackStep
{
    WARMUP,
    FIRE,
    COOLDOWN,
    DONE
}

public class Captain : BaseEnemy
{
    public float TargetXPos;
    public Transform PlayerTransform;

    public float FireChance = 0.05f;
    public AttackStep attackStep = AttackStep.DONE;

    public float[] AttackPhaseTimes;

    private float nextAttackPhaseTime;
    private bool MoveUp;

    protected override void Start()
    {
        base.Start();
        TargetXPos = transform.position.x;
        MoveUp = false;
        if (Random.value < 0.5f)
            MoveUp = true;
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        switch (State)
        {
            case AnimState.ATTACK:
                switch (attackStep)
                {
                    case AttackStep.WARMUP:
                        if (Time.time > nextAttackPhaseTime)
                            attackStep = AttackStep.FIRE;
                        break;
                    case AttackStep.FIRE:
                        Debug.Log("BANG!");
                        // fire the projectile here!
                        nextAttackPhaseTime = Time.time + AttackPhaseTimes[1];
                        attackStep = AttackStep.COOLDOWN;
                        break;
                    case AttackStep.COOLDOWN:
                        if (Time.time > nextAttackPhaseTime)
                            attackStep = AttackStep.DONE;
                        break;
                    case AttackStep.DONE:
                        ChangeAnimState(AnimState.IDLE);
                        break;
                }
                break;
        }
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

    private void DoAttack()
    {
        attackStep = AttackStep.WARMUP;
        nextAttackPhaseTime = Time.time + AttackPhaseTimes[0];
        ChangeAnimState(AnimState.ATTACK);
    }
}
