using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightningDamage : NetworkBehaviour
{

    public bool enemy_check;
    public int damage_val;
    public GameObject damage_values;
    public Transform impact_point;

    private Collider2D[] check_colliders;
    //private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();

    private Animator anim;
    [SyncVar] public bool lightning_start;
    [SyncVar] public bool lightning_middle1;
    [SyncVar] public bool lightning_middle2;
    [SyncVar] public bool lightning_end;

    private string check_tag;
    private Vector2 above_target;

    private string player_name;
    private GameObject player;

    public int damage_int_value;

    // Use this for initialization
    void Start ()
    {
        collider_filter = collider_filter.NoFilter();

        anim = gameObject.GetComponent<Animator>();

        anim.SetBool("lightning_start", lightning_start);
        anim.SetBool("lightning_middle1", lightning_middle1);
        anim.SetBool("lightning_middle2", lightning_middle2);
        anim.SetBool("lightning_end", lightning_end);

        if (!enemy_check)
        {
            check_tag = "BOT_Enemy";
        }
        else if (enemy_check)
        {
            check_tag = "BOT_Player";
        }

        if (isServer)
        {
            player_name = "Player1";
        }
        else
        {
            player_name = "Player2";
        }

        //Debug.Log(enemy_check);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnTriggerEnter2D(Collider2D Target)
    {
        player = GameObject.Find(player_name);

        if (Target.tag == check_tag)
        {
            //Debug.Log(check_colliders[checkVar]);

            Target.gameObject.GetComponent<Plating>().DamagePlating(damage_val);

            above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

            if (damage_val > Target.gameObject.GetComponent<Plating>().armor_value)
            {
                damage_int_value = (damage_val - Target.gameObject.GetComponent<Plating>().armor_value);
            }
            if (damage_val <= Target.gameObject.GetComponent<Plating>().armor_value)
            {
                damage_int_value = 1;
            }

            player.GetComponent<PlayerSpawnScript>().CmdSpawnDamageValues(damage_values, above_target, damage_int_value);
        }
    }

    /*
    
        player = GameObject.Find(player_name);

        check_colliders = new Collider2D[scan_size];

        for (int setVar = 0; setVar < scan_size; setVar++)
        {
            check_colliders[setVar] = null;
        }

        gameObject.GetComponent<Collider2D>().OverlapCollider(collider_filter, check_colliders);

        for (int checkVar = 0; checkVar < scan_size; checkVar++)
        {
            if (check_colliders[checkVar] != null)
            {
                if (check_colliders[checkVar].tag == check_tag)
                {
                    //Debug.Log(check_colliders[checkVar]);

                    check_colliders[checkVar].gameObject.GetComponent<Plating>().DamagePlating(damage_val);

                    above_target = new Vector2(check_colliders[checkVar].transform.position.x, check_colliders[checkVar].transform.position.y + 50);

                    if (damage_val > check_colliders[checkVar].gameObject.GetComponent<Plating>().armor_value)
                    {
                        damage_int_value = (damage_val - check_colliders[checkVar].gameObject.GetComponent<Plating>().armor_value);
                    }
                    if (damage_val <= check_colliders[checkVar].gameObject.GetComponent<Plating>().armor_value)
                    {
                        damage_int_value = 1;
                    }

                    player.GetComponent<PlayerSpawnScript>().CmdSpawnDamageValues(damage_values, above_target, damage_int_value);
                }
            }
        }

    */

    public void DestroySelf()
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

}
