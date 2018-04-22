using UnityEngine;
using System.Collections;

public enum AttackStep
{
    WARMUP,
    FIRE,
    COOLDOWN,
    DONE
}

public class ProjectileEnemy : BaseEnemy
{
    public Transform PlayerTransform;
    public Transform BulletSpawnPoint;
    public GameObject BulletPrefab;
    public float FireChance = 0.05f;
    public AttackStep attackStep = AttackStep.DONE;

    public float[] AttackPhaseTimes;

    protected float nextAttackPhaseTime;

    protected override void Start()
    {
        base.Start();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        if (Active & gameController.State == GameState.RUNNING)
        {
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
                            GameObject projectileObject = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.identity);
                            Projectile projectile = projectileObject.GetComponent<Projectile>();
                            projectile.Velocity *= Facing;
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
    }

    protected void DoAttack()
    {
        attackStep = AttackStep.WARMUP;
        nextAttackPhaseTime = Time.time + AttackPhaseTimes[0];
        ChangeAnimState(AnimState.ATTACK);
    }
}
