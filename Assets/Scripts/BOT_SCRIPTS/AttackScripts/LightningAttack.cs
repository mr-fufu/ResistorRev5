using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LightningAttack : MonoBehaviourPunCallbacks
{
    public bool in_range;
    public GameObject range_detector;

    public int attack_speed;
    public int lightning_damage;

    public GameObject lightning_object;
    private LightningRange range;

    public float lightning_length;

    public Transform launch_point;
    private float reloadtime = 100;
    public bool enemy_check;
    public int power;
    public int logic;

    [PunRPC]
    public void SyncLightningAttack(bool isEnemy)
    {
        enemy_check = isEnemy;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            range = range_detector.GetComponent<LightningRange>();

            var parentStatBlock = transform.parent.transform.parent.GetComponent<StandardStatBlock>();
            logic = parentStatBlock.LOGIC;
            power = parentStatBlock.POWER;
            lightning_damage = power * 4;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            reloadtime -= attack_speed * Time.deltaTime * 50 * 0.65f;

            if (reloadtime <= 0)
            {
                in_range = logic > 0 ? range.scanned : true;

                if (in_range)
                {
                    BattleFactorySpawn.instance.SpawnLightning(lightning_object, launch_point.gameObject, lightning_damage, enemy_check, power);

                    reloadtime = 100;
                }
            }
        }
    }
}
