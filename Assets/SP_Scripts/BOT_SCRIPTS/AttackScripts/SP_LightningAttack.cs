using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SP_LightningAttack : NetworkBehaviour
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

    private float lightning_dist;
    private Vector3 lightning_point;
    private bool alternator;
    public bool spawned;
    public bool attached;

    private string player_name;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        attached = gameObject.GetComponent<PartStats>().attached;

        if (isServer)
        {
            player_name = "Player1";
        }
        else
        {
            player_name = "Player2";
        }

    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find(player_name);

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
                    player.GetComponent<PlayerSpawnScript>().CmdSpawnLightning(lightning_object, launch_point.gameObject, lightning_damage, enemy_check, power);

                    reloadtime = 100;
                }
            }
        }
    }
}
