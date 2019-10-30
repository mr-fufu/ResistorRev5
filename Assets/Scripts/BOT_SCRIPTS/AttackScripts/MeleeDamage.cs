using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour {

    public bool enemy_check;
    public bool attached;
    public int damage_val;

    public GameObject sparks_object;
    public GameObject damage_values;
    public Transform impact_point;
    private bool spawned;
    private Vector2 above_target;

    public bool root;
    public bool leg;
    public bool body;

    // Use this for initialization
    void Start() {
        //Debug.Log(enemy_check);
    }

    // Update is called once per frame
    void Update() {
        damage_val = transform.parent.GetComponent<MeleeAttack>().damage_val;

        if (root)
        {
            attached = GetComponent<PartStats>().attached;
            enemy_check = transform.parent.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
            damage_val = transform.parent.GetComponent<MeleeAttack>().damage_val * transform.parent.parent.gameObject.GetComponent<StandardStatBlock>().SPEED;
        }
        else if (body || leg)
        {
            attached = transform.parent.GetComponent<PartStats>().attached;
            enemy_check = transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
        }
        else
        {
            attached = transform.parent.GetComponent<PartStats>().attached;
            enemy_check = transform.parent.parent.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
        }

    }

    void OnTriggerEnter2D(Collider2D Target)
    {
        if (!enemy_check && Target.gameObject.tag == "BOT_Enemy")
        {
            Target.gameObject.GetComponent<Plating>().DamagePlating(damage_val);
            Instantiate(sparks_object, impact_point.position, impact_point.rotation);

            above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

            var clone = (GameObject) Instantiate(damage_values, above_target, Quaternion.Euler(Vector3.zero));

            if (damage_val > Target.GetComponent<Plating>().armor_value)
            {
                clone.GetComponent<DamageValues>().damage_value = (damage_val - Target.GetComponent<Plating>().armor_value);
            }
            if (damage_val <= Target.GetComponent<Plating>().armor_value)
            {
                clone.GetComponent<DamageValues>().damage_value = 1;
            }
        }
        else if (enemy_check && Target.gameObject.tag == "BOT_Player")
        {   
            Target.gameObject.GetComponent<Plating>().DamagePlating(damage_val);
            Instantiate(sparks_object, impact_point.position, impact_point.rotation);

            above_target = new Vector2(Target.transform.position.x, Target.transform.position.y + 50);

            var clone = (GameObject)Instantiate(damage_values, above_target, Quaternion.Euler(Vector3.zero));

            if (damage_val > Target.GetComponent<Plating>().armor_value)
            {
                clone.GetComponent<DamageValues>().damage_value = (damage_val - Target.GetComponent<Plating>().armor_value);
            }
            if (damage_val <= Target.GetComponent<Plating>().armor_value)
            {
                clone.GetComponent<DamageValues>().damage_value = 1;
            }
        }
    }
}
