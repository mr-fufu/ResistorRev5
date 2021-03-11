using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StandardStatBlock : MonoBehaviourPunCallbacks
{

    // The Standard Stat Block (SSB) script is used as a container for
    // the *overall* stats of the bot. Stats of SSB are only ever transmitted (changed) down (from child to parent).
    // for SSB's that are not attached to the root (LEG part) the stats are pulled up from parent part. E.g. LEG part
    // contains SSB that contains overall stats, TORSO part will update its stats to match that of LEG part and then Hydra
    // Front Part will update its stats to match that of the TORSO.

    // Stats will only actually change if a part is added or removed (usually in the workshop scene through the workshop
    // drag script or when bots are built in the battle scene). 

    // Standard Stat Block also contains additional details about the bot such as whether any of its parts use fuel 
    // (generally reserved for flame attack weapons) as well as whether any of the parts do melee damage (melee_components)
    // which functions as a count. *NOTE* fuel used has been moved to the dedicated Fueling Script and fuel management
    // code has been commented out.

    //public bool fuel_used;
    //public bool fuel_instantiated;
    //public int fuel_remaining;
    //private float fuel_clock;
    //public GameObject fuel_bar;
    //public GameObject fuel_object;

    public bool workshop_show_piece;

    public int melee_components = 0;

    private AutoMove autoMove;
    private bool useStat;
    private StandardStatBlock statBlock;

    // for overloaded setters
    private int _plate, _range, _speed, _power;
    public int PLATE{ get => _plate; private set => _plate = Math.Max(1, value); }
    public int LOGIC{ get; private set; }
    public int RANGE{ get => _range; private set => _range = Math.Max(1, value); }
    public int ARMOR{ get; private set; }
    public int SPEED{ get => _speed; private set => _speed = Math.Max(1, value); }
    public int FUEL{ get; private set; }
    public int POWER{ get => _power; private set => _power = Math.Max(1, value); }
    public int COST{ get; private set; }
    public bool ENEMY{ get; set; }
    
    [PunRPC]
    public void SyncStats(int newPlate, int newLogic, int newRange, int newArmor, 
        int newSpeed, int newFuel, int newPower, int newCost)
    {
        PLATE = newPlate;
        LOGIC = newLogic;
        RANGE = newRange;
        ARMOR = newArmor;
        SPEED = newSpeed;
        FUEL = newFuel;
        POWER = newPower;
        COST = newCost;

        var plating = GetComponent<Plating>();
        if (plating != null)
        {
            plating.InitializePlating(PLATE, ARMOR);
        }
    }

    [PunRPC]
    public void SyncEngage(bool engaged)
    {
        engaged_check = engaged;
    }

    public bool spawned;
    public bool engaged_check;

    private bool attached;
    private string part_type;

    private GameObject double_parent_object;
    private GameObject quadruple_parent_object;

    // Use this for initialization
    void Start()
    {
        spawned = true;
        //fuel_remaining = 100;

        // get part_type and attached from attached PartStats script

        part_type = gameObject.GetComponent<PartStats>().part_type;

        attached = gameObject.GetComponent<PartStats>().attached;

        // Once attached, set the double and quadruple parent accordingly (double used for
        // both TORSO and ARMOR components attached to LEG parts and quadruple used only if ARMOR is attached to 
        // a TORSO part.

        if (!workshop_show_piece)
        {
            if (photonView.IsMine)
            {
                if (part_type == "TORSO" || part_type == "ARMOR")
                {
                    double_parent_object = gameObject.transform.parent.transform.parent.gameObject;
                }
                if (part_type == "ARMOR")
                {
                    if (double_parent_object.GetComponent<PartStats>().part_type == "TORSO")
                    {
                        {
                            quadruple_parent_object = double_parent_object.transform.parent.transform.parent.gameObject;
                        }
                    }
                }
            }
        }

        CheckAutoMove();
    }

    // Update is called once per frame
    void Update()
    {
        // if the part is a leg, check for spawned and engaged_check from the Automove Script.

        if (workshop_show_piece)
        {
            attached = false;
            UpdateStats();
        }

        if (PLATE <= 1 && !workshop_show_piece)
        {
            attached = true;
            UpdateStats();
        }

        if (!workshop_show_piece)
        {
            if (photonView.IsMine)
            {
                if (autoMove == null && statBlock == null)
                {
                    CheckAutoMove();
                }
                else
                {
                    if (useStat)
                    {
                        engaged_check = statBlock.engaged_check;
                    }
                    else
                    {
                        engaged_check = autoMove.engaged_check;
                    }
                }

                GetComponent<PhotonView>().RPC("SyncEngage", RpcTarget.All, engaged_check);
            }
        }

        // once spawned, set the minimum values for stats that cannot be 0
        // i.e. bot cannot have zero health or speed (though this should be impossible
        // as all LEG parts have a minimum of 10 plate and 1 speed. Range should be
        // at least 1 as all ranged weapons currently have a range of minimum 1 (though
        // this may be subject to change so additional weapons do not increase the range of
        // a bot. Power is set to minimum 1 since otherwise lightning weapons will not
        // function correctly. Similar to range as it may be changed in the future, though
        // additional tesla weapons increasing lightning range and damage may be more
        // acceptable aesthetically

        // ^ moved to private setters

    }

    public void UpdateStats()
    {
        // Pull Stats from double or quadruple objects according to part type (specified in initialization start() of
        // this script as well. Also, for battle scene spawned bots, pull the spawned and engaged bools in adition to stats

        if (ENEMY == PhotonNetwork.IsMasterClient)
            return;

        if (attached)
        {
            if (part_type == "TORSO")
            {
                var doubleStatBlock = double_parent_object.GetComponent<StandardStatBlock>();
                PLATE = doubleStatBlock.PLATE;
                LOGIC = doubleStatBlock.LOGIC;
                RANGE = doubleStatBlock.RANGE;
                ARMOR = doubleStatBlock.ARMOR;
                SPEED = doubleStatBlock.SPEED;
                FUEL = doubleStatBlock.FUEL;
                POWER = doubleStatBlock.POWER;
                //ENEMY = doubleStatBlock.ENEMY;

                if (!workshop_show_piece)
                {
                    GetComponent<PhotonView>().RPC("SyncStats", RpcTarget.All,
                        PLATE, LOGIC, RANGE, ARMOR, SPEED, FUEL, POWER, COST);
                }
            }
            else if (part_type == "ARMOR")
            {
                if (double_parent_object.GetComponent<PartStats>().part_type == "TORSO")
                {
                    var quadStatBlock = quadruple_parent_object.GetComponent<StandardStatBlock>();
                    PLATE = quadStatBlock.PLATE;
                    LOGIC = quadStatBlock.LOGIC;
                    RANGE = quadStatBlock.RANGE;
                    ARMOR = quadStatBlock.ARMOR;
                    SPEED = quadStatBlock.SPEED;
                    FUEL = quadStatBlock.FUEL;
                    POWER = quadStatBlock.POWER;
                    //ENEMY = quadStatBlock.ENEMY;

                    if (!workshop_show_piece)
                    {
                        GetComponent<PhotonView>().RPC("SyncStats", RpcTarget.All,
                            PLATE, LOGIC, RANGE, ARMOR, SPEED, FUEL, POWER, COST);
                    }
                }
                else if (double_parent_object.GetComponent<PartStats>().part_type == "LEG")
                {
                    var doubleStatBlock = double_parent_object.GetComponent<StandardStatBlock>();
                    PLATE = doubleStatBlock.PLATE;
                    LOGIC = doubleStatBlock.LOGIC;
                    RANGE = doubleStatBlock.RANGE;
                    ARMOR = doubleStatBlock.ARMOR;
                    SPEED = doubleStatBlock.SPEED;
                    FUEL = doubleStatBlock.FUEL;
                    POWER = doubleStatBlock.POWER;
                    //ENEMY = doubleStatBlock.ENEMY;

                    if (!workshop_show_piece)
                    {
                        GetComponent<PhotonView>().RPC("SyncStats", RpcTarget.All,
                            PLATE, LOGIC, RANGE, ARMOR, SPEED, FUEL, POWER, COST);
                    }
                }
            }
        }
    }

    // Transmission functions. Each simply passes the function down to the double parent (skipping the parent
    // since it will be a part slot). TORSO passes to LEG (always) while ARMOR may pass to LEG or TORSO in which
    // case the later will cause the TORSO to then pass the value to the LEG. Only when the function arrives at the
    // LEG will the SSB stat value change (will be increased by the transmitted value).

    public void transmitStats(int newPlate, int newLogic, int newRange, int newArmor, 
        int newSpeed, int newFuel, int newPower, int newCost)
    {

        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            var torsoStatBlock = transform.parent.transform.parent.GetComponent<StandardStatBlock>();
            
            torsoStatBlock.PLATE += newPlate;
            torsoStatBlock.LOGIC += newLogic;
            torsoStatBlock.RANGE += newRange;
            torsoStatBlock.ARMOR += newArmor;
            torsoStatBlock.SPEED += newSpeed;
            torsoStatBlock.FUEL += newFuel;
            torsoStatBlock.POWER += newPower;
            torsoStatBlock. COST += newCost;
        }
        else if (part_type == "ARMOR")
        {
            var torsoStatBlock = transform.parent.transform.parent.GetComponent<StandardStatBlock>();
            torsoStatBlock.transmitStats(newPlate, newLogic, newRange, newArmor, newSpeed, newFuel, newPower, newCost);
        }
        else if (part_type == "LEG")
        {
            PLATE += newPlate;
            LOGIC += newLogic;
            RANGE += newRange;
            ARMOR += newArmor;
            SPEED += newSpeed;
            FUEL += newFuel;
            POWER += newPower;
            COST += newCost;
        }

        if (!workshop_show_piece)
        {
            GetComponent<PhotonView>().RPC("SyncStats", RpcTarget.All,
                PLATE, LOGIC, RANGE, ARMOR, SPEED, FUEL, POWER, COST);
        }
    }
    
    public void transmit_melee(int melee_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().melee_components += melee_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_melee(melee_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().melee_components += melee_val;
        }
    }

    void CheckAutoMove()
    {
        if (!workshop_show_piece)
        {
            if (photonView.IsMine)
            {
                if (part_type == "LEG")
                {
                    useStat = false;
                    autoMove = GetComponent<AutoMove>();
                }
                else if (part_type == "TORSO")
                {
                    useStat = true;
                    statBlock = double_parent_object.GetComponent<StandardStatBlock>();
                }
                else if (part_type == "ARMOR")
                {
                    if (double_parent_object.GetComponent<PartStats>().part_type == "TORSO")
                    {
                        useStat = true;
                        statBlock = quadruple_parent_object.GetComponent<StandardStatBlock>();
                    }
                    else if (double_parent_object.GetComponent<PartStats>().part_type == "LEG")
                    {
                        useStat = false;
                        autoMove = double_parent_object.GetComponent<AutoMove>();
                    }
                }
            }
        }
    }
}
