using UnityEngine;
using System.Collections;

public enum GameState
{
    RUNNING,
    SCROLLING,
    PAUSED,
}

public class GameController : MonoBehaviour
{
    public GameObject[] SpawnPrefabs;
    public Transform Camera;

    public int ScreenWidth = 320;
    public int CurrentPosition = 160;
    public int TargetPosition = 0;
    public int ScrollAmount = 296;
    public int ScrollPerFrame = 2;

    public GameState State;

    // Use this for initialization
    void Start()
    {
        ActivateEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject toSpawn = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            toSpawn = Instantiate(SpawnPrefabs[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            toSpawn = Instantiate(SpawnPrefabs[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            toSpawn = Instantiate(SpawnPrefabs[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            toSpawn = Instantiate(SpawnPrefabs[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            toSpawn = Instantiate(SpawnPrefabs[4]);
        }

        if (toSpawn)
        {
            toSpawn.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            toSpawn.GetComponent<BaseActor>().Active = true;
            toSpawn.GetComponent<BaseActor>().LeftBound += CurrentPosition - 160;
            toSpawn.GetComponent<BaseActor>().RightBound += CurrentPosition - 160;
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            CanScroll();
        }
    }

    private void FixedUpdate()
    {
        if (State == GameState.SCROLLING)
        {
            CurrentPosition += ScrollPerFrame;
            Camera.position = new Vector3(CurrentPosition, Camera.position.y, Camera.position.z);
            if (CurrentPosition == TargetPosition)
                State = GameState.RUNNING;
        }
    }
    public bool CanScroll()
    {
        // find all active enemies
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy.Active)
            {
                Debug.Log("Found active enemy, can't scroll");
                return false;
            }
        }
        // if none - can scroll
        Debug.Log("No active enemies");
        return true;
    }

    public bool StartScroll()
    {
        if (!CanScroll())
            return false;

        // destroy all projectiles
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject);
        }

        // set state to scrolling
        State = GameState.SCROLLING;

        // calculate new position
        TargetPosition = CurrentPosition + ScrollAmount;

        // activate new enemies
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            enemy.LeftBound += ScrollAmount;
            enemy.RightBound += ScrollAmount;
            if (enemy.transform.position.x >= TargetPosition & enemy.transform.position.x <= TargetPosition + ScreenWidth)
            {
                enemy.Active = true;
            }
        }
        return true;
    }

    private void ActivateEnemies()
    {
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            if (enemy.transform.position.x >= TargetPosition & enemy.transform.position.x <= TargetPosition + ScreenWidth)
            {
                enemy.Active = true;
            }
        }
    }
}
