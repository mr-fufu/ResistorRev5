using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightningDamage : MonoBehaviourPunCallbacks
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

    public string check_tag;
    private Vector2 above_target;

    public int damage_int_value;

    [PunRPC]
    public void SyncLightning(bool isEnemy, bool l_start, bool l_mid_1, bool l_mid_2, bool l_end, int damage, int power)
    {
        enemy_check = isEnemy;

        lightning_start = l_start;
        lightning_middle1 = l_mid_1;
        lightning_middle2 = l_mid_2;
        lightning_end = l_end;

        damage_val = damage;

        var collider = GetComponent<BoxCollider2D>();

        collider.enabled = l_start;

        collider.size = new Vector2((power + 2) * 26, 60);
        collider.offset = new Vector2(13 + 13 * (power), 10);

        //Debug.LogError("Damage Set to " + damage);
    }

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

        //Debug.LogError("Current Tag to check is " + check_tag);
    }

    void OnTriggerEnter2D(Collider2D Target)
    {
        if (Target.tag == check_tag)
        {
            if (photonView.IsMine)
            {
                Target.GetComponent<Plating>().DamagePlatingLightning(damage_val);

                above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

                if (damage_val > Target.gameObject.GetComponent<Plating>().armor_value)
                {
                    damage_int_value = (damage_val - Target.GetComponent<Plating>().armor_value);
                }
                else
                {
                    damage_int_value = 1;
                }

                BattleFactorySpawn.instance.SpawnDamagePopUpNetwork(damage_values, above_target, damage_int_value);
            }
        }
    }

    public void destroy_trigger()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
