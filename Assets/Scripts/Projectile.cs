using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float Velocity;
    public GameController gameController;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void FixedUpdate()
    {
        if (gameController.State == GameState.RUNNING)
        {
            transform.position += new Vector3(Velocity, 0, 0);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            Destroy(gameObject);
    }
}
