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

    public int ScreenWidth = 320;
    public int CurrentPosition = 0;
    public int ScrollAmount = 296;
    public int ScrollPerFrame = 2;

    public GameState State;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(SpawnPrefabs[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(SpawnPrefabs[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Instantiate(SpawnPrefabs[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Instantiate(SpawnPrefabs[3]);
        }
    }

    public bool CanScroll()
    {
        return false;
    }

    public bool StartScroll()
    {
        return false;
    }
}
