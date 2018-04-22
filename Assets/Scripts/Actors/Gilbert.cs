using UnityEngine;
using System.Collections;

public class Gilbert : BaseEnemy
{
    public float TargetXPos;

    public GameObject[] SpawnEnemies;
    public float SpawnTime;
    public float SpawnX;
    public int SpawnYMin = 8;
    public int SpawnYMax = 96;

    private bool MoveUp;

    private float? nextSpawnTime;

    protected override void Start()
    {
        base.Start();
        TargetXPos = transform.position.x;
        MoveUp = false;
        if (Random.value < 0.5f)
            MoveUp = true;
    }

    private void FixedUpdate()
    {

        if (Active & gameController.State == GameState.RUNNING)
        {
            Vector3 moveVector = Vector3.zero;
            if (State == AnimState.HIT)
            {
                nextSpawnTime = null;
                Facing = -hitDirection;
                moveVector = new Vector3(HitStaggerSpeed, 0, 0) * hitDirection;
            }

            if (State == AnimState.IDLE | State == AnimState.WALK)
            {
                if (nextSpawnTime == null)
                {
                    nextSpawnTime = Time.time + SpawnTime;
                }
                if (transform.position.y >= UpperBound)
                {
                    MoveUp = false;
                }
                else if (transform.position.y <= LowerBound)
                {
                    MoveUp = true;
                }

                if (MoveUp)
                {
                    moveVector += new Vector3(0, WalkSpeed, 0);
                }
                else
                {
                    moveVector -= new Vector3(0, WalkSpeed, 0);
                }

                // check if we're at target x-pos
                if (transform.position.x != TargetXPos)
                {
                    // if no, move towards x-pos diagonally
                    if (transform.position.x > TargetXPos)
                    {
                        moveVector -= new Vector3(WalkSpeed, 0, 0);
                        Facing = -1;
                    }
                    else
                    {
                        moveVector = new Vector3(WalkSpeed, 0, 0);
                        Facing = 1;
                    }
                }
                ChangeAnimState(AnimState.WALK);

                if (Time.time > nextSpawnTime)
                {
                    DoAttack();
                }
            }

            if (State == AnimState.ATTACK)
            {
                // to get out of ATTACK, just spam it and let the base class handle it.
                ChangeAnimState(AnimState.IDLE);
            }

            transform.position += moveVector;
        }
    }

    private void DoAttack()
    {
        // TODO: play whistle
        ChangeAnimState(AnimState.ATTACK);
        int SpawnIndex = Random.Range(0, SpawnEnemies.Length);
        int SpawnY = Random.Range(SpawnYMin, SpawnYMax);
        GameObject newEnemy = Instantiate(SpawnEnemies[SpawnIndex], new Vector3(SpawnX, SpawnY, SpawnY), Quaternion.identity);
        BaseActor newActor = newEnemy.GetComponent<BaseActor>();
        newActor.Active = true;
        newActor.LeftBound += 272;
        newActor.RightBound += 272;
        
        nextSpawnTime = Time.time + SpawnTime;
    }
}
