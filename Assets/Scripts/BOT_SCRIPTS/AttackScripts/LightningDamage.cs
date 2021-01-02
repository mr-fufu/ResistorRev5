using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningDamage : MonoBehaviour
{

    public bool enemy_check;
    public int damage_val;
    public GameObject damage_values;
    public Transform impact_point;

    private Collider2D[] check_colliders;
    //private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();

    private Animator anim;

    // TODO SAM: These were sync vars
    public bool lightning_start;
    public bool lightning_middle1;
    public bool lightning_middle2;
    public bool lightning_end;

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
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnTriggerEnter2D(Collider2D Target)
    {
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

            BattleFactorySpawn.instance.SpawnDamagePopUp(damage_values, above_target, damage_int_value);
        }
    }
}
