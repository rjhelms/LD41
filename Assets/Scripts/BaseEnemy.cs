using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseActor
{
    #region Public variables
    [Header("More sprites")]
    public Sprite HitSprite;
    public Sprite DeadSprite;

    [Header("Hit Stats")]
    public float HitStaggerTime = 0.5f;
    public float DeadStaggerTime = 0.5f;
    public float DeadFlashTime = 0.1f;

    public float HitStaggerSpeed = 2.0f;

    [Header("Collision Avoidance")]
    public bool HangFrame = false;
    public float HangFrameChance = 0.8f;
    #endregion

    #region Protected variables
    protected int hitDirection = 1;
    protected float hitStaggerEnd = 0f;
    protected float deadStaggerEnd = 0f;
    protected float deadFlashNext = 0f;
    #endregion

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Active & gameController.State == GameState.RUNNING)
        {
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
        }
        else
        {
            ChangeAnimState(AnimState.DEAD);
            deadStaggerEnd = Time.time + DeadStaggerTime;
            deadFlashNext = Time.time + DeadFlashTime;
        }
    }
    
    protected virtual void OnCollisionStay2D(Collision2D collision)
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
