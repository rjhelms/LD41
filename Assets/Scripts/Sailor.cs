using UnityEngine;
using System.Collections;

public class Sailor : BaseActor
{
    public int HitPoints = 1;
    public Transform PlayerTransform;

    public float TrackChance = 0.5f;
    protected override void Start()
    {
        base.Start();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        Vector3 moveVector = TrackPlayer() * WalkSpeed;
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
            transform.position += moveVector;
        }
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

    public override void Hit()
    {
        Debug.Log("Ouch!");
        HitPoints--;
        if (HitPoints == 0)
        {
            Destroy(gameObject);
        }
    }
}
