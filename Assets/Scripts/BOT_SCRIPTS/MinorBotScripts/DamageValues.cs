using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DamageValues : NetworkBehaviour {

    public float rise_speed = 1;
    [SyncVar] public int damage_value;
    public Text displayed_value;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (rise_speed > 0)
        {
            rise_speed -= 0.01f;
        }
        else
        {
            rise_speed = 0;
        }

        displayed_value.text = "-" + damage_value;
        transform.position = new Vector3(transform.position.x, transform.position.y + (rise_speed), transform.position.z);
	}
}
