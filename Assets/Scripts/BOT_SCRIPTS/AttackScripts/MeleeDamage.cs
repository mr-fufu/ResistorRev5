using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeleeDamage : MonoBehaviourPunCallbacks
{
    public string searchTag;
    public bool enemy_check;
    public int damage_val;
    public int damage_int_value;

    public GameObject sparks_object;
    public GameObject damage_values;
    public Transform impact_point;
    private Vector2 above_target;

    public bool root;
    public bool leg;
    public bool body;

    private MeleeAttack _meleeAttack;
    
    void Start()
    {
        _meleeAttack = transform.parent.GetComponent<MeleeAttack>();

        if (photonView.IsMine)
        {
            if (root)
            {
                enemy_check = transform.parent.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
                damage_val = transform.parent.GetComponent<MeleeAttack>().damage_val * transform.parent.parent.gameObject.GetComponent<StandardStatBlock>().SPEED;
            }
            else if (body || leg)
            {
                enemy_check = transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
            }
            else
            {
                enemy_check = transform.parent.parent.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
            }

            damage_val = _meleeAttack.damage_val;

            searchTag = enemy_check ? "BOT_Player" : "BOT_Enemy";
        }
    }

    void OnTriggerEnter2D(Collider2D Target)
    {
        if (Target.gameObject.tag == searchTag)
        {
            if (photonView.IsMine)
            {
                Target.gameObject.GetComponent<Plating>().DamagePlating(damage_val);

                above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

                damage_int_value = 1;

                if (damage_val > Target.GetComponent<Plating>().armor_value)
                {
                    damage_int_value = (damage_val - Target.GetComponent<Plating>().armor_value);
                }
                if (damage_val <= Target.GetComponent<Plating>().armor_value)
                {
                    damage_int_value = 1;
                }

                BattleFactorySpawn.instance.SpawnDamagePopUpNetwork(damage_values, above_target, damage_int_value);
                BattleFactorySpawn.instance.SpawnImpact(true, sparks_object, impact_point.position, impact_point.rotation, true, gameObject, enemy_check);
            }
        }
    }
}
