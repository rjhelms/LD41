using UnityEngine;
using System.Collections;

public class Sailor : BaseEnemy
{
    #region public variables
    [Header("Player tracking")]
    public float TrackChance = 0.5f;
    public Transform PlayerTransform;

    [Header("Attack properites")]
    public float AttackDistance;
    public float AttackChance;
    public Collider2D PunchCollider;
    #endregion

    protected override void Start()
    {
        base.Start();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;
        if (State < AnimState.HIT & HangFrame == false)
        {
            if (State < AnimState.ATTACK)
            {
                Vector2 distance = (Vector2)transform.position - (Vector2)PlayerTransform.position;
                if (Mathf.Abs(distance.magnitude) < AttackDistance)
                {
                    if (Random.value < AttackChance)
                        DoAttack();
                }
            }

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
    
    private void DoAttack()
    {
        // logic for punching goes here
        PunchCollider.enabled = true;
        ChangeAnimState(AnimState.ATTACK);
    }
}
