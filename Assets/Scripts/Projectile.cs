using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float Velocity;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(Velocity, 0, 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
            Destroy(gameObject);
    }
}
