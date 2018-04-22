using UnityEngine;
using System.Collections;

public class Sailor : BaseEnemy
{
    #region public variables
    [Header("Player tracking")]
    public float TrackChance = 0.5f;
    public Transform PlayerTransform;
    public Vector2 TrackRandomVector;
    public int TrackRandomness;
    public float TrackRandomTime;

    [Header("Attack properites")]
    public float AttackDistance;
    public float AttackChance;
    public Collider2D PunchCollider;
    #endregion

    private float nextTrackRandomTime;
    protected override void Start()
    {
        base.Start();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateTrackRandomness();
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time > nextTrackRandomTime)
        {
            GenerateTrackRandomness();
        }
    }
    private void FixedUpdate()
    {
        if (Active & gameController.State == GameState.RUNNING)
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
    }

    private Vector3 TrackPlayer()
    {
        Vector2 targetPosition = new Vector2(PlayerTransform.position.x, PlayerTransform.position.z);
        targetPosition += TrackRandomVector;
        if (targetPosition.x < LeftBound)
        {
            targetPosition += new Vector2(LeftBound - targetPosition.x, 0);
        } else if (targetPosition.x > RightBound)
        {
            targetPosition += new Vector2(RightBound - targetPosition.x, 0);
        }
        if (targetPosition.y > UpperBound)
        {
            targetPosition += new Vector2(0, UpperBound - targetPosition.y);
        } else if (targetPosition.y < LowerBound)
        {
            targetPosition += new Vector2(0, LowerBound - targetPosition.y);
        }
        int xTrack = 0;
        int yTrack = 0;
        if (targetPosition.x < transform.position.x)
            xTrack = -1;
        if (targetPosition.x > transform.position.x)
            xTrack = 1;

        if (Random.value >= TrackChance)
            xTrack = 0;

        // track player Z, to follow the base of the jump
        if (targetPosition.y < transform.position.y)
            yTrack = -1;
        if (targetPosition.y > transform.position.y)
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
        // TODO: Enemy punch sound
    }

    private void GenerateTrackRandomness()
    {
        Vector2 randomVector = Random.insideUnitCircle * TrackRandomness;
        TrackRandomVector = new Vector2(Mathf.RoundToInt(randomVector.x), Mathf.RoundToInt(randomVector.y));
        nextTrackRandomTime = Time.time + TrackRandomTime;
    }
}
