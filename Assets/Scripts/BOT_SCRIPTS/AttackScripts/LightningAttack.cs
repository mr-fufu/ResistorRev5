using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAttack : MonoBehaviour
{
    public bool in_range;
    public GameObject range_detector;

    public int attack_speed;
    public int lightning_damage;

    public GameObject lightning_object;

    public float lightning_length;

    public Transform launch_point;
    private float reloadtime = 100;
    private bool enemy_check;
    public int power;

    public bool spawned;
    public bool attached;


    void Start()
    {
        attached = gameObject.GetComponent<PartStats>().attached;
    }

    void Update()
    {

        if (attached)
        {
            spawned = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().spawned;
        }

        if (spawned)
        {
            enemy_check = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
            power = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().POWER;
            lightning_damage = power * 4;
        }

        if (spawned)
        {

            reloadtime -= attack_speed * Time.deltaTime * 50 * 0.65f;
            if (reloadtime <= 0)
            {

                if (transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC > 0)
                {
                    in_range = range_detector.GetComponent<LightningRange>().scanned;
                }
                else
                {
                    in_range = true;
                }

                if (in_range)
                {
                    BattleFactorySpawn.instance.SpawnLightning(lightning_object, launch_point.gameObject, lightning_damage, enemy_check, power);

                    reloadtime = 100;
                }
            }
        }
    }
}
