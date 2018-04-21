using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseActor
{
    public Sprite HitSprite;
    public Sprite DeadSprite;

    public Transform PlayerTransform;

    public float TrackChance = 0.5f;
    public float HitStaggerTime = 0.5f;
    public float DeadStaggerTime = 0.5f;
    public float DeadFlashTime = 0.1f;

    public float HitStaggerSpeed = 2.0f;

    protected int hitDirection = 1;
    protected float hitStaggerEnd = 0f;
    protected float deadStaggerEnd = 0f;
    protected float deadFlashNext = 0f;

    public bool HangFrame = false;
    public float HangFrameChance = 0.8f;

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
            case AnimState.DEAD:
                spriteRenderer.sprite = DeadSprite;
                if (Time.time > deadStaggerEnd)
                {
                    Destroy(gameObject);
                }
                if (Time.time > deadFlashNext)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                    deadFlashNext += DeadFlashTime;
                }
                break;

        }
    }
}
