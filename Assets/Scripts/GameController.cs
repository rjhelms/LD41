using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject[] SpawnPrefabs;
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
}
