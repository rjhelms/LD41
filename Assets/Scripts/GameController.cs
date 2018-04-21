using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject SailorPrefab;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(SailorPrefab);
        }
    }
}
