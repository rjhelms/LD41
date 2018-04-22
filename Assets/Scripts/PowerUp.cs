using UnityEngine;
using System.Collections;

public enum PowerUpType
{
    HEALTH,
    SPEED,
    BOMB,
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType Type;
    public int ScoreValue;
    public int PowerUpValue;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
