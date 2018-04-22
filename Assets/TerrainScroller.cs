using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScroller : MonoBehaviour {

    public int ScrollAmount = 360;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
        {
            if (transform.position.x < collision.transform.position.x)
            {
                Debug.Log("Off the edge!");
                transform.position += new Vector3(ScrollAmount, 0, 0);
            }
        }
    }
}
