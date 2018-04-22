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
    public int AttackShots = 1;
    public float[] AttackPhaseTimes;

    public AudioClip ProjectileSound;

    protected float nextAttackPhaseTime;
    protected int currentShot = 0;
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
                            {
                                attackStep = AttackStep.FIRE;
                                currentShot = 0;
                            }
                            break;
                        case AttackStep.FIRE:
                            GameObject projectileObject = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.identity);
                            Projectile projectile = projectileObject.GetComponent<Projectile>();
                            projectile.Velocity *= Facing;
                            nextAttackPhaseTime = Time.time + AttackPhaseTimes[1];
                            attackStep = AttackStep.COOLDOWN;
                            currentShot++;
                            break;
                        case AttackStep.COOLDOWN:
                            if (Time.time > nextAttackPhaseTime)
                            {
                                if (currentShot == AttackShots)
                                {
                                    attackStep = AttackStep.DONE;
                                } else
                                {
                                    attackStep = AttackStep.FIRE;
                                }
                            }
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
        gameController.Audio.PlayOneShot(ProjectileSound);
    }
}
