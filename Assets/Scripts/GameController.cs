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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject gameObject = Instantiate(SpawnPrefabs[0]);
            gameObject.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            gameObject.GetComponent<BaseActor>().Active = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject gameObject = Instantiate(SpawnPrefabs[1]);
            gameObject.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            gameObject.GetComponent<BaseActor>().Active = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject gameObject = Instantiate(SpawnPrefabs[2]);
            gameObject.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            gameObject.GetComponent<BaseActor>().Active = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject gameObject = Instantiate(SpawnPrefabs[3]);
            gameObject.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            gameObject.GetComponent<BaseActor>().Active = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameObject gameObject = Instantiate(SpawnPrefabs[4]);
            gameObject.transform.position += new Vector3(CurrentPosition - 160, 0, 0);
            gameObject.GetComponent<BaseActor>().Active = true;
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
