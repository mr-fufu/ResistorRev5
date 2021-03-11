 using System.Collections;
using System.Collections.Generic;
 using Photon.Pun;
 using UnityEngine;

public class ProjectileAttack : MonoBehaviourPunCallbacks
{

    // Attached to a bot part that can fire a projectile prefab from a given launch point(s)

    // double and multi-attack public bools, checked true for certain parts. Double attack requires
    // 2 launch points (launch_point and launch_point_2) and multi-attack requires all 4 launch-points.
    // attacks will alternate between launch points sequentially. Launch number set appropriately
    public bool double_attack;
    public bool multi_attack;

    private int launch_number;

    public float attack_speed;
    public float variable_attack_speed_modifier;
    public GameObject projectile;
    private float reloadtime = 100;
    public bool enemy_check;

    [PunRPC]
    public void SyncIsEnemyForProjectiles(bool isEnemy)
    {
        enemy_check = isEnemy;
    }

    public Transform launch_point;
    public Transform launch_point_2;
    public Transform launch_point_3;
    public Transform launch_point_4;

    public Transform[] launch_points;

    public bool spawned;
    public bool attached;

    public bool variable_attack_speed;

    public bool variable_range;
    public int variable_range_value;
    public bool range_stat_dependent;

    public bool variable_speed;
    public int variable_speed_value;

    public bool variable_y;
    public float variable_y_value;

    public bool variable_damage;
    public int variable_damage_value;

    public bool projectile_parent;
    public bool attack_over;

    public bool out_of_fuel;
    public bool uses_fuel;
    public int fuel_use;
    private int fuel_remaining;

    public bool use_scan;
    public bool logic_dependent;
    public bool scanned;
    public GameObject range_detector;

    public bool uses_flare;
    public GameObject flare_object;

    private Vector2 launch_location;

    private int index;

    public GameObject player;

    public GameObject leg_component;

    void Start()
    {
        spawned = true;
        // set the launch_points array to include the appropriate launch points
        // Also set the launch number to the correct value
        if (double_attack)
        {
            launch_number = 2;

            launch_points = new Transform[2];

            launch_points[0] = launch_point;
            launch_points[1] = launch_point_2;
        }
        else if (multi_attack)
        {
            launch_number = 4;

            launch_points = new Transform[4];

            launch_points[0] = launch_point;
            launch_points[1] = launch_point_2;
            launch_points[2] = launch_point_3;
            launch_points[3] = launch_point_4;
        }
        else
        {
            launch_number = 1;

            launch_points = new Transform[1];

            launch_points[0] = launch_point;
        }

        // check whether the bot part the projectile attack script is attached to has been attached to 
        // another bot part
        attached = gameObject.GetComponent<PartStats>().attached;

        // check whether the range is dependent on and affected by the RANGE stat of the bot
        if (range_stat_dependent)
        {
            variable_range = true;
            variable_range_value = 0;
        }

        // find leg object function to search for the leg part of the bot
        find_leg_object(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // find the fuel remaining value (similar to plating) on the leg bot part of the robot and set fuel_used bool to true
            // to instantiate the fuel bar. Check the fuel remaining and set out_of_fuel bool accordingly.
            if (uses_fuel)
            {
                leg_component.GetComponent<Fueling>().fuel_used = true;

                fuel_remaining = leg_component.GetComponent<Fueling>().fuel_remaining;

                if (fuel_remaining < 1)
                {
                    out_of_fuel = true;
                }
                else
                {
                    out_of_fuel = false;
                }
            }

            // if the robot constantly fires without needing to have a target in range then set scanned to always be true. Otherwise,
            // if the robot has a LOGIC of at least 1 then the robot will only fire when a target is in range.

            if (!use_scan)
            {
                scanned = true;
            }
            else if (logic_dependent)
            {
                if (transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC > 0)
                {
                    // if the weapon can use scan and the overall LOGIC of the bot is at least 1 then only fire if the
                    // range detector detects an opposing bot
                    scanned = range_detector.GetComponent<RangeDetection>().scanned;
                }
                else
                {
                    // if the weapon can use scan but the overall LOGIC of the bot is 0, then always fire (dont scan)
                    scanned = true;
                }
            }
            else
            {
                // otherwise, if the weapon uses scan but is not logic dependent then always use the range detector
                scanned = range_detector.GetComponent<RangeDetection>().scanned;
            }

            // check to see whether the bot is an ENEMY (belongs to player 2 or the player on the right side of the screen)
            // by checking the SSB of the double parent (since single parent is the slot component and the double parent is
            // the part being attached to)
            //enemy_check = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;

            // if the weapon has scanned an opposing bot (or if scanned is always true and the projectile attack doesn't or cannot use scan)
            if (scanned)
            {
                // if the weapon is not out of fuel (stops fire if fuel is out)
                if (!out_of_fuel)
                {
                    // if the attack speed is not constant then check the SSB for LOGIC and use that to set reloadtime. Otherwise, reloadtime is 
                    // affected only by the attack_speed (a public int set by the prefab as a stat inherent to the weapon)
                    if (variable_attack_speed)
                    {
                        reloadtime -=
                            (attack_speed * 0.5f +
                             transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC *
                             variable_attack_speed_modifier) * Time.deltaTime * 50;
                    }
                    else
                    {
                        reloadtime -= attack_speed * 0.5f * Time.deltaTime * 50;
                    }

                    // if the reloadtime reaches 0 or less then fire the weapon and reset reloadtime
                    if (reloadtime <= 0)
                    {
                        // launch location is set to launch point of index. cycles through the launch points for double and multi attack, otherwise
                        // always set to the same launch point and launch_points is an array of size 1 only
                        launch_location = launch_points[index].position;


                        //TODO SAM : handle random across server, seed it?

                        // variable y is a public bool set by the prefab as an inherent weapon stat that modifies the height between shots
                        // by a small random amount based on the variable y value mostly for aesthetic value for certain wapons (i.e. minigun)
                        if (variable_y)
                        {
                            launch_location[1] += Random.Range(-variable_y_value, variable_y_value);
                        }

                        // if the weapon uses fuel then transmit the fuel used by the projectile to the leg component Fueling script (similar to plating)
                        if (uses_fuel)
                        {
                            leg_component.GetComponent<Fueling>().transmit_fuel(-fuel_use);
                        }

                        BattleFactorySpawn.instance.SpawnProjectile(projectile, launch_location, gameObject);

                        // spawns a flare (muzzle flare) gameobject at the point of launch (usually static and destroys itself after animation plays)
                        if (uses_flare)
                        {
                            BattleFactorySpawn.instance.SpawnGeneric(flare_object, launch_location, gameObject,
                                gameObject.transform.rotation);
                        }

                        // index increases after each shot
                        index++;

                        // reset the index to 0 after it reaches the number of shots (2 for double attack and 4 for multi-attack)
                        // index increases before check so cycles from 0 to 1 for double and 0 to 3 for multi and 0 to 0 for single attack

                        // reload times are affected slightly for multi and double attack. Each shot results in a reload time of 50 for double,
                        // each shot results in a reload time of 100 for single and multi attack reloads after 5 seconds except when it reaches the
                        // maximum number of shots fired for a long reload. Multi attack functions as firing 4 shots in quick succession followed by a 
                        // long reload (mostly for aesthetic purposes)
                        if (index == launch_number)
                        {
                            index = 0;

                            if (double_attack)
                            {
                                reloadtime = 50;
                            }
                            else if (multi_attack)
                            {
                                reloadtime = 80;
                            }
                            else
                            {
                                reloadtime = 100;
                            }
                        }
                        else if (double_attack)
                        {
                            reloadtime = 50;
                        }
                        else if (multi_attack)
                        {
                            reloadtime = 5;
                        }
                    }
                }
            }
        }
    }

    // find leg object function: checks each parent gameobject for a PartStats script and if the part type is not LEG then
    // check parent (check exists for if parent exists) for find_leg_object. Records leg gameobject as leg_component otherwise
    // if leg is not found record null (should not happen in normal circumstances since spawning parents instantiated parts as
    // children and workshop build begins by putting down a leg part
    void find_leg_object(GameObject find_object)
    {
        if (find_object.GetComponent<PartStats>() != null)
        {
            if (find_object.GetComponent<PartStats>().part_type == "LEG")
            {
                leg_component = find_object;
            }
            else
            {
                if (find_object.transform.parent != null)
                {
                    if (find_object.transform.parent.parent != null)
                    {
                        find_leg_object(find_object.transform.parent.parent.gameObject);
                    }
                    else
                    {
                        leg_component = null;
                    }
                }
                else
                {
                    leg_component = null;
                }
            }
        }
        else
        {
            leg_component = null;
        }
    }
}
