using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DamageValues : MonoBehaviourPunCallbacks
{
    public int damage_value;

    public float rise_speed = 1;
    
    public Text displayed_value;

    [PunRPC]
    public void SyncDamageValue (int value)
    {
        damage_value = value;
    }


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
