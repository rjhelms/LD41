using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseActor {

    #region Public variables
    [Header("Colliders")]
    public Collider2D PunchCollider;
    public Collider2D JumpCollider;

    [Header("Hit Stats")]
    public float HitInvulnTime = 0.5f;
    public float HitFlashTime = 0.1f;
    public float BombCoolDownTime = 0.5f;
    public float DeadStaggerTime = 0.5f;
    public float DeadInvulnTime = 1f;

    public bool IsInvulnerable;
    #endregion

    #region Private variables
    private float hitInvulnEnd;
    private float hitNextFlash;
    protected float deadStaggerEnd = 0f;
    protected float deadFlashNext = 0f;
    private float nextBombCooldownTime = 0f;
    #endregion

    // Update is called once per frame
    protected override void Update () {
        if (Active & gameController.State == GameState.RUNNING) 
        {
            if (State != AnimState.DEAD)
            {
                // punch
                if (Input.GetButtonDown("Fire1"))
                {
                    if (State < AnimState.ATTACK)
                    {
                        DoAttack();
                    }
                }

                // jump
                if (Input.GetButtonDown("Fire2"))
                {
                    if (State < AnimState.JUMP)
                    {
                        DoJump();
                    }
                }
                if (Input.GetButtonDown("Fire3"))
                {
                    if (ScoreManager.Instance.Bombs > 0 & Time.time > nextBombCooldownTime)
                    {
                        DoBomb();
                    }
                }
            }
            base.Update();
            if (State == AnimState.DEAD)
            {
                spriteRenderer.sprite = DeadSprite;
            }
            if (IsInvulnerable)
            {
                if (Time.time > hitNextFlash)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                    hitNextFlash += HitFlashTime;
                }
                if (State != AnimState.DEAD & Time.time > hitInvulnEnd)
                {
                    EndHit();
                } else if (State == AnimState.DEAD & Time.time > deadStaggerEnd)
                {
                    ChangeAnimState(AnimState.IDLE);
                    ScoreManager.Instance.HitPoints = 3;
                    hitInvulnEnd = Time.time + DeadInvulnTime;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        WalkSpeed = ScoreManager.Instance.WalkSpeed;
        if (Active & gameController.State == GameState.RUNNING & State != AnimState.DEAD)
        {
            AnimState newState = AnimState.IDLE;
            Vector3 moveVector = new Vector3(0, 0, 0);
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (transform.position.x < RightBound)
                    moveVector += new Vector3(1, 0, 0) * WalkSpeed;
                else CheckRightBound();
                newState = AnimState.WALK;
                Facing = 1;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (transform.position.x > LeftBound)
                    moveVector -= new Vector3(1, 0, 0) * WalkSpeed;
                newState = AnimState.WALK;
                Facing = -1;
            }

            // process jump frame
            if (IsJumping)
            {
                moveVector += new Vector3(0, JumpCurrentSpeed, 0);
                if (JumpAccelFrame)
                {
                    JumpCurrentSpeed -= 1;
                    if (JumpCurrentSpeed < 0)
                        JumpCollider.enabled = true;
                }
                if (JumpCurrentSpeed < -JumpStartSpeed)
                {
                    JumpCollider.enabled = false;
                    IsJumping = false;
                }
                JumpAccelFrame = !JumpAccelFrame;
            }


            if (State != AnimState.JUMP) // can't move vert during jump
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    if (transform.position.y < UpperBound)
                    {
                        moveVector += new Vector3(0, 1, 0) * WalkSpeed;
                        newState = AnimState.WALK;
                    }
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    if (transform.position.y > LowerBound)
                    {
                        moveVector -= new Vector3(0, 1, 0) * WalkSpeed;
                        newState = AnimState.WALK;
                    }
                }
            }

            if (Time.time > AttackEndTime)
            {
                PunchCollider.enabled = false;
            }

            ChangeAnimState(newState);
            transform.position += moveVector;
        }
    }

    private void CheckRightBound()
    {
        if (gameController.StartScroll())
        {
            LeftBound += gameController.ScrollAmount;
            RightBound += gameController.ScrollAmount;
        }
    }
    
    private void DoAttack()
    {
        // logic for punching goes here
        PunchCollider.enabled = true;
        ChangeAnimState(AnimState.ATTACK);
        // TODO: Player attack sound
    }

    private void DoJump()
    {
        ChangeAnimState(AnimState.JUMP);
        JumpCurrentSpeed = JumpStartSpeed;
        JumpAccelFrame = false;
        IsJumping = true;
        // TODO: Player jump sound
    }

    private void DoBomb()
    {
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy.Active & enemy.State < AnimState.HIT)
            {
                if (enemy.transform.position.x < transform.position.x)
                {
                    enemy.Hit(-1);
                } else
                {
                    enemy.Hit(1);
                }
            }
        }
        ScoreManager.Instance.Bombs--;
        nextBombCooldownTime = Time.time + BombCoolDownTime;
        // TODO: Player bomb sound
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Enemy")
        {
            BaseActor enemy = collision.gameObject.GetComponent<BaseActor>();
            if (collision.otherCollider == PunchCollider | collision.otherCollider == JumpCollider)
            {
                enemy.Hit(Facing);
            }
            else if (!IsJumping | JumpCurrentSpeed >= 0)
            {
                if (enemy.State != AnimState.HIT & enemy.State != AnimState.DEAD)
                    Hit(Facing);
            }
        } else if (collision.collider.gameObject.tag == "EnemyProjectile")
        {
            if (collision.otherCollider != PunchCollider & collision.otherCollider != JumpCollider)
            {
                Hit(Facing);
                Destroy(collision.collider.gameObject);
            }
        } if (collision.gameObject.tag == "PowerUp")
        {
            PowerUp powerUp = collision.gameObject.GetComponent<PowerUp>();
            switch (powerUp.Type)
            {
                // TODO: powerup sound
                case PowerUpType.BOMB:
                    ScoreManager.Instance.Bombs++;
                    break;
                case PowerUpType.HEALTH:
                    if (ScoreManager.Instance.HitPoints < ScoreManager.Instance.MaxHitPoints)
                        ScoreManager.Instance.HitPoints++;
                    break;
                case PowerUpType.SPEED:
                    if (ScoreManager.Instance.WalkSpeed < ScoreManager.Instance.MaxWalkSpeed)
                        ScoreManager.Instance.WalkSpeed++;
                    break;
            }
            ScoreManager.Instance.Score += powerUp.ScoreValue;
            Destroy(collision.gameObject);
        }
    }

    public override void Hit(int direction)
    {
        if (!IsInvulnerable)
        {
            IsInvulnerable = true;
            ScoreManager.Instance.HitPoints--;
            if (ScoreManager.Instance.HitPoints == 0)
            {
                Die();
            }
            // TODO: player hit sound
            hitInvulnEnd = Time.time + HitInvulnTime;
            hitNextFlash = Time.time + HitFlashTime;
        }
    }

    protected override void EndHit()
    {
        spriteRenderer.enabled = true;
        IsInvulnerable = false;
    }

    private void Die()
    {
        IsInvulnerable = true;
        deadStaggerEnd = Time.time + DeadStaggerTime;
        hitNextFlash = Time.time + HitFlashTime;
        ScoreManager.Instance.Lives -= 1;
        if (ScoreManager.Instance.Lives >= 0)
        {
            ChangeAnimState(AnimState.DEAD);
            // TODO: Player dead sound
        }
        else
        {
            gameController.LoseLevel();
        }
    }
}
