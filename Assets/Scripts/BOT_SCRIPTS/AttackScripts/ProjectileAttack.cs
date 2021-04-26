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
    
    public bool doubleAttack;
    public bool multiAttack;

    private int launchNumber;

    public float attackSpeed;
    public bool variableAttackSpeed;
    public float variableAttackSpeedValue;

    public GameObject projectile;
    private float reloadtime = 100;

    public List<Transform> launchPoints;

    public bool variableRange;
    public bool variableRangeUsesStat;
    public int variableRangeValue;

    public bool variableSpeed;
    public int variableSpeedValue;

    public bool variableHeight;
    public float variableHeightValue;

    public bool variableDamage;
    public int variableDamageValue;

    public bool attackOver;

    private bool outOfFuel;
    public bool usesFuel;
    private int fuelRemaining;

    public bool useScan;
    public bool logicScan;
    private bool scanned;

    public GameObject rangeDetector;
    public GameObject flareObject;
    
    private Vector2 launchLocation;
    private int index;

    [System.NonSerialized] public bool enemyCheck;
    [System.NonSerialized] public bool projectileParent;
    [System.NonSerialized] public GameObject leg_component;

    [PunRPC]
    public void SyncIsEnemyForProjectiles(bool isEnemy)
    {
        enemyCheck = isEnemy;
    }


    void Start()
    {
        // set the launch_points array to include the appropriate launch points
        // Also set the launch number to the correct value
        if (doubleAttack)
        {
            launchNumber = 2;
        }
        else if (multiAttack)
        {
            launchNumber = 4;
        }
        else
        {
            launchNumber = 1;
        }

        // check whether the range is dependent on and affected by the RANGE stat of the bot
        if (variableRangeUsesStat)
        {
            variableRange = true;
            variableRangeValue = 0;
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
            if (usesFuel)
            {
                leg_component.GetComponent<Fueling>().fuel_used = true;

                fuelRemaining = leg_component.GetComponent<Fueling>().fuel_remaining;

                if (fuelRemaining < 1)
                {
                    outOfFuel = true;
                }
                else
                {
                    outOfFuel = false;
                }
            }

            // if the robot constantly fires without needing to have a target in range then set scanned to always be true. Otherwise,
            // if the robot has a LOGIC of at least 1 then the robot will only fire when a target is in range.

            if (!useScan)
            {
                scanned = true;
            }
            else if (logicScan)
            {
                if (transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC > 0)
                {
                    // if the weapon can use scan and the overall LOGIC of the bot is at least 1 then only fire if the
                    // range detector detects an opposing bot
                    scanned = rangeDetector.GetComponent<RangeDetection>().scanned;
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
                scanned = rangeDetector.GetComponent<RangeDetection>().scanned;
            }

            // check to see whether the bot is an ENEMY (belongs to player 2 or the player on the right side of the screen)
            // by checking the SSB of the double parent (since single parent is the slot component and the double parent is
            // the part being attached to)
            //enemy_check = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;

            // if the weapon has scanned an opposing bot (or if scanned is always true and the projectile attack doesn't or cannot use scan)
            if (scanned)
            {
                // if the weapon is not out of fuel (stops fire if fuel is out)
                if (!outOfFuel)
                {
                    // if the attack speed is not constant then check the SSB for LOGIC and use that to set reloadtime. Otherwise, reloadtime is 
                    // affected only by the attack_speed (a public int set by the prefab as a stat inherent to the weapon)
                    if (variableAttackSpeed)
                    {
                        reloadtime -=
                            (attackSpeed * 0.5f +
                             transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC *
                             variableAttackSpeedValue) * Time.deltaTime * 50;
                    }
                    else
                    {
                        reloadtime -= attackSpeed * 0.5f * Time.deltaTime * 50;
                    }

                    // if the reloadtime reaches 0 or less then fire the weapon and reset reloadtime
                    if (reloadtime <= 0)
                    {
                        // launch location is set to launch point of index. cycles through the launch points for double and multi attack, otherwise
                        // always set to the same launch point and launch_points is an array of size 1 only
                        launchLocation = launchPoints[index].position;


                        //TODO SAM : handle random across server, seed it?

                        // variable y is a public bool set by the prefab as an inherent weapon stat that modifies the height between shots
                        // by a small random amount based on the variable y value mostly for aesthetic value for certain wapons (i.e. minigun)
                        if (variableHeight)
                        {
                            launchLocation[1] += Random.Range(-variableHeightValue, variableHeightValue);
                        }

                        // if the weapon uses fuel then transmit the fuel used by the projectile to the leg component Fueling script (similar to plating)
                        if (usesFuel)
                        {
                            leg_component.GetComponent<Fueling>().transmit_fuel(-1);
                        }

                        BattleFactorySpawn.instance.SpawnProjectile(projectile, launchLocation, gameObject);

                        // spawns a flare (muzzle flare) gameobject at the point of launch (usually static and destroys itself after animation plays)
                        if (flareObject != null)
                        {
                            BattleFactorySpawn.instance.SpawnGeneric(flareObject, launchLocation, gameObject,
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
                        if (index == launchNumber)
                        {
                            index = 0;

                            if (doubleAttack)
                            {
                                reloadtime = 50;
                            }
                            else if (multiAttack)
                            {
                                reloadtime = 80;
                            }
                            else
                            {
                                reloadtime = 100;
                            }
                        }
                        else if (doubleAttack)
                        {
                            reloadtime = 50;
                        }
                        else if (multiAttack)
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
            if (find_object.GetComponent<PartStats>().partType == "LEG")
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
