using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StandardStatBlock : MonoBehaviour {

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

    public int PLATE;
    public int LOGIC;
    public int RANGE;
    public int ARMOR;
    public int SPEED;
    public int FUEL;
    public int POWER;
    public int AGILITY;
    public int COST;
    public bool ENEMY;

    [PunRPC]
    public void SyncIsEnemy(bool isEnemy)
    {
        ENEMY = isEnemy;
    }

    public bool spawned;
    public bool engaged_check;

    private bool attached;
    private string part_type;

    private GameObject double_parent_object;
    private GameObject quadruple_parent_object;

    // Use this for initialization
    void Start () {

        spawned = true;
        //fuel_remaining = 100;

        // get part_type and attached from attached PartStats script

        part_type = gameObject.GetComponent<PartStats>().part_type;

        attached = gameObject.GetComponent<PartStats>().attached;

        // Once attached, set the double and quadruple parent accordingly (double used for
        // both TORSO and ARMOR components attached to LEG parts and quadruple used only if ARMOR is attached to 
        // a TORSO part.

        if (attached)
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
	
	// Update is called once per frame
	void Update ()
    {
        // if the part is a leg, check for spawned and engaged_check from the Automove Script.

        if (part_type == "LEG")
        {
            if (!workshop_show_piece)
            {
                engaged_check = gameObject.GetComponent<AutoMove>().engaged_check;
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

            if (spawned)
            {
                if (PLATE < 1)
                {
                    PLATE = 1;
                }

                if (SPEED < 1)
                {
                    SPEED = 1;
                }

                if (RANGE < 1)
                {
                    RANGE = 1;
                }

                if (POWER < 1)
                {
                    POWER = 1;
                }
            }
        }

        // Pull Stats from double or quadruple objects according to part type (specified in initialization start() of
        // this script as well. Also, for battle scene spawned bots, pull the spawned and engaged bools in adition to stats

        if (attached)
        {
            if (part_type == "TORSO")
            {
                PLATE = double_parent_object.GetComponent<StandardStatBlock>().PLATE;
                LOGIC = double_parent_object.GetComponent<StandardStatBlock>().LOGIC;
                RANGE = double_parent_object.GetComponent<StandardStatBlock>().RANGE;
                ARMOR = double_parent_object.GetComponent<StandardStatBlock>().ARMOR;
                SPEED = double_parent_object.GetComponent<StandardStatBlock>().SPEED;
                FUEL = double_parent_object.GetComponent<StandardStatBlock>().FUEL;
                POWER = double_parent_object.GetComponent<StandardStatBlock>().POWER;
                AGILITY = double_parent_object.GetComponent<StandardStatBlock>().AGILITY;
                ENEMY = double_parent_object.GetComponent<StandardStatBlock>().ENEMY;

                if (!workshop_show_piece)
                {
                    engaged_check = double_parent_object.GetComponent<AutoMove>().engaged_check;
                }
            }
            else if (part_type == "ARMOR")
            {
                if (double_parent_object.GetComponent<PartStats>().part_type == "TORSO")
                {
                    PLATE = quadruple_parent_object.GetComponent<StandardStatBlock>().PLATE;
                    LOGIC = quadruple_parent_object.GetComponent<StandardStatBlock>().LOGIC;
                    RANGE = quadruple_parent_object.GetComponent<StandardStatBlock>().RANGE;
                    ARMOR = quadruple_parent_object.GetComponent<StandardStatBlock>().ARMOR;
                    SPEED = quadruple_parent_object.GetComponent<StandardStatBlock>().SPEED;
                    FUEL = quadruple_parent_object.GetComponent<StandardStatBlock>().FUEL;
                    POWER = quadruple_parent_object.GetComponent<StandardStatBlock>().POWER;
                    AGILITY = quadruple_parent_object.GetComponent<StandardStatBlock>().AGILITY;
                    ENEMY = quadruple_parent_object.GetComponent<StandardStatBlock>().ENEMY;

                    if (!workshop_show_piece)
                    {
                        engaged_check = quadruple_parent_object.GetComponent<AutoMove>().engaged_check;
                    }
                }
                else if (double_parent_object.GetComponent<PartStats>().part_type == "LEG")
                {
                    PLATE = double_parent_object.GetComponent<StandardStatBlock>().PLATE;
                    LOGIC = double_parent_object.GetComponent<StandardStatBlock>().LOGIC;
                    RANGE = double_parent_object.GetComponent<StandardStatBlock>().RANGE;
                    ARMOR = double_parent_object.GetComponent<StandardStatBlock>().ARMOR;
                    SPEED = double_parent_object.GetComponent<StandardStatBlock>().SPEED;
                    FUEL = double_parent_object.GetComponent<StandardStatBlock>().FUEL;
                    POWER = double_parent_object.GetComponent<StandardStatBlock>().POWER;
                    AGILITY = double_parent_object.GetComponent<StandardStatBlock>().AGILITY;
                    ENEMY = double_parent_object.GetComponent<StandardStatBlock>().ENEMY;

                    if (!workshop_show_piece)
                    {
                        engaged_check = double_parent_object.GetComponent<AutoMove>().engaged_check;
                    }
                }
            }
        }
    }

    // Transmission functions. Each simply passes the function down to the double parent (skipping the parent
    // since it will be a part slot). TORSO passes to LEG (always) while ARMOR may pass to LEG or TORSO in which
    // case the later will cause the TORSO to then pass the value to the LEG. Only when the function arrives at the
    // LEG will the SSB stat value change (will be increased by the transmitted value).

    //-----------------------------------------------------------------------------

    public void transmit_PLATE(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().PLATE += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_PLATE(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().PLATE += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_LOGIC(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().LOGIC += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_LOGIC(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().LOGIC += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_RANGE(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().RANGE += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_RANGE(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().RANGE += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_ARMOR(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().ARMOR += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_ARMOR(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().ARMOR += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_SPEED(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().SPEED += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_SPEED(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().SPEED += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_FUEL(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().FUEL += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_FUEL(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().FUEL += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_POWER(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().POWER += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_POWER(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().POWER += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_FRAME(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().AGILITY += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_FRAME(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().AGILITY += transmit_val;
        }
    }

    //-----------------------------------------------------------------------------

    public void transmit_COST(int transmit_val)
    {
        part_type = gameObject.GetComponent<PartStats>().part_type;

        if (part_type == "TORSO")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().COST += transmit_val;
        }
        else if (part_type == "ARMOR")
        {
            transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_COST(transmit_val);
        }
        else if (part_type == "LEG")
        {
            gameObject.GetComponent<StandardStatBlock>().COST += transmit_val;
        }
    }

    // I used to calculate fuel remaining in the SSB but a bot should only have one part calculating fuel and both
    // LEG and TORSO have SSB component. Therefore, fueling and all fuel related code has been moved to a separate
    // script called Fueling(similar to plating) that is exclusively attached to LEG-type bot parts.

    //public void transmit_fuel_remaining(int fuel_val)
    //{
    //    part_type = gameObject.GetComponent<PartStats>().part_type;

    //    if (part_type == "TORSO")
    //    {
    //        transform.parent.transform.parent.GetComponent<StandardStatBlock>().fuel_remaining += fuel_val;
    //    }
    //    else if (part_type == "ARMOR")
    //    {
    //        transform.parent.transform.parent.GetComponent<StandardStatBlock>().transmit_fuel_remaining(fuel_val);
    //    }
    //    else if (part_type == "LEG")
    //    {
    //        gameObject.GetComponent<StandardStatBlock>().fuel_remaining += fuel_val;
    //    }
    //}

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
}
