using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

    private StandardStatBlock _standardStatBlock;


    void Start()
    {
        attached = gameObject.GetComponent<PartStats>().attached;
        _standardStatBlock = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>();;
    }

    void Update()
    {

        if (attached)
        {
            spawned = _standardStatBlock.spawned;
        }

        if (spawned)
        {
            enemy_check = _standardStatBlock.ENEMY;
            power = _standardStatBlock.POWER;
            lightning_damage = power * 4;

            reloadtime -= attack_speed * Time.deltaTime * 50 * 0.65f;
            if (reloadtime <= 0)
            {

                if (_standardStatBlock.LOGIC > 0)
                {
                    in_range = range_detector.GetComponent<LightningRange>().scanned;
                }
                else
                {
                    in_range = true;
                }

                if (in_range)
                {
                    if (enemy_check != PhotonNetwork.IsMasterClient)
                    {
                        BattleFactorySpawn.instance.SpawnLightning(lightning_object, launch_point.gameObject, lightning_damage, enemy_check, power);
                    }
                    reloadtime = 100;
                }
            }
        }
    }
}
