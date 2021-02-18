using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{

    public int projectile_speed;
    public bool enemy_check;
    public int damage_val;

    public bool random_impact;
    public int random_chance;

    public bool use_impact;
    public GameObject impact_object;
    public GameObject damage_values;
    public Transform impact_point;
    public float y_velocity;

    public bool piercing;

    private string search_tag;

    private int move_dir = 0;
    private Vector2 above_target;

    public GameObject player;

    public int damage_int_value;

    public float ground_y_position;

    void Start () {
        if (PhotonNetwork.IsMasterClient)
        {
            player = GameObject.Find("Player1");
        }
        else
        {
            player = GameObject.Find("Player2");
        }

        if (!enemy_check)
        {
            search_tag = "BOT_Enemy";
        }
        else if (enemy_check)
        {
            search_tag = "BOT_Player";
        }

        move_dir = 1;
    }
	
	void Update () {
        transform.Translate(new Vector3(projectile_speed * move_dir * 0.25f* Time.deltaTime * 50, y_velocity, 0f));
	}

    void OnTriggerEnter2D(Collider2D Target)
    {
        if (Target.gameObject.tag == search_tag)
        {
            Target.gameObject.GetComponent<Plating>().DamagePlating(damage_val);

            above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

            if (damage_val > Target.GetComponent<Plating>().armor_value)
            {
                damage_int_value = (damage_val - Target.GetComponent<Plating>().armor_value);
            }
            if (damage_val <= Target.GetComponent<Plating>().armor_value)
            {
                damage_int_value = 1;
            }

            BattleFactorySpawn.instance.SpawnDamagePopUp(damage_values, above_target, damage_int_value);
            BattleFactorySpawn.instance.SpawnImpact(use_impact, impact_object, impact_point.position, impact_point.rotation, piercing, gameObject, enemy_check);
        }
    }

    public void destroy_trigger()
    {
        if (enemy_check != PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
