using UnityEngine;
using System.Collections;

public class BaseEnemy : BaseActor
{
    #region Public variables
    [Header("More sprites")]
    public Sprite HitSprite;


    [Header("Hit Stats")]
    public float HitStaggerTime = 0.5f;
    public float DeadStaggerTime = 0.5f;
    public float DeadFlashTime = 0.1f;

    public float HitStaggerSpeed = 2.0f;

    [Header("Collision Avoidance")]
    public bool HangFrame = false;
    public float HangFrameChance = 0.8f;

    [Header("PowerUps")]
    public int ScoreValueHit = 100;
    public int ScoreValueKill = 500;
    public GameObject[] PowerUps;
    public float[] SpawnChance;
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
                        if (PowerUps.Length > 0)
                        {
                            float PowerUpValue = Random.value;
                            for (int i = 0; i < PowerUps.Length; i++)
                            {
                                if (PowerUpValue <= SpawnChance[i])
                                {
                                    Vector3 spawnPosition = transform.position;
                                    while (spawnPosition.x < (gameController.CurrentPosition - 144))
                                    {
                                        spawnPosition += new Vector3(8, 0, 0);
                                    }
                                    Instantiate(PowerUps[i], spawnPosition, Quaternion.identity);
                                    i = PowerUps.Length;
                                }
                            }
                        }
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
        // TODO: Enemy hit sound
        HitPoints--;
        ChangeAnimState(AnimState.HIT);
        hitDirection = direction;
        hitStaggerEnd = Time.time + HitStaggerTime;
        rigidbody2D.simulated = false;
        ScoreManager.Instance.Score += ScoreValueHit;
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
            ScoreManager.Instance.Score += ScoreValueKill;
            ChangeAnimState(AnimState.DEAD);
            deadStaggerEnd = Time.time + DeadStaggerTime;
            deadFlashNext = Time.time + DeadFlashTime;
            // TODO: enemy dead sound
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
