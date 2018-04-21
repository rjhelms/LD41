using UnityEngine;
using System.Collections;

public class Sailor : BaseActor
{
    public Sprite HitSprite;
    public int HitPoints = 1;
    public Transform PlayerTransform;

    public float TrackChance = 0.5f;
    public float HitStaggerTime = 0.5f;
    public float DeadStaggerTime = 0.5f;
    public float DeadFlashTime = 0.1f;

    public float HitStaggerSpeed = 2.0f;

    private int hitDirection = 1;
    private float hitStaggerEnd = 0f;
    private float deadStaggerEnd = 0f;
    private float deadFlashNext = 0f;

    protected override void Start()
    {
        base.Start();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        switch (State)
        {
            case AnimState.HIT:
                spriteRenderer.sprite = HitSprite;
                if (Time.time > hitStaggerEnd)
                    EndHit();
                break;
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;
        if (State < AnimState.HIT)
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
        hitStaggerEnd = Time.time + HitStaggerTime;
        rigidbody2D.simulated = false;
        if (HitPoints == 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void EndHit()
    {
        ChangeAnimState(AnimState.IDLE);
        rigidbody2D.simulated = true;
    }
}
