using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PartStats : MonoBehaviourPunCallbacks
{
    // Part stats hold all of the associated stats of a bot part. Part stats scripts
    // are only ever attached to bot parts and bot part slots. Both the showcase and actual spawned game
    // bot parts contain the part stats script. This is because showcase parts will
    // display the stats of the robot to be spawned in battle and the spawned bots
    // need bot parts to calculate overall stats

    // Bot stats are as given below. PLATE is used by plating and determines bot health(or plating)
    // LOGIC increases a bot's weapon capabilities, allowing it to fire slightly faster and also
    // allows certain weapons to conserve ammo by only firing when enemy bots are in range. RANGE
    // affects certain weapons (usually bullets and missiles) and allows projectiles to travel
    // further. ARMOR reduces incoming damage by the amount of overall ARMOR. Speed affects how
    // quickly bots move. FUEL is a specific stat only used for certain  weapons (Flamethrowers).
    // POWER affects the range and damage of tesla weapons. COST is the amount of credits
    // that the bot part costs. 

    // All bot part stats are passed down to the LEG bot part through the transmit function. All parts
    // that can have other parts attached to them will have a Standard Stat Block script (or SSB). Calling
    // the transmit function present in the SSB will send a value that will only change the SSB's stats
    // if the SSB is attached to the LEG part and will otherwise transmit downwards until an SSB is 
    // found that is attached to a LEG. The SSB will then copy values upwards. For more detail, see the
    // comments on the transmit function in the SSB script.

    // Transmit functions are only called when parts are attached or detached in the Workshop through
    // the Workshop Drag script. Thus stats only change when parts are attached or detached.

    public int PLATE;
    public int LOGIC;
    public int RANGE;
    public int ARMOR;
    public int SPEED;
    public int FUEL;
    public int POWER;
    public int FRAME;
    public int COST;
    public bool ENEMY;

    // A pseudo-stat, the melee_component simply transmits down to tell the bot whether
    // a melee part is currently attached (usually an arm). The bool causes the LEG part's
    // collider to be slightly smaller and allows the bot to get slightly closer to the enemy bot

    public bool meleeComponent;

    // (IMPORTANT) the go to check for whether a part is a slot or a part. Slot components use
    // a slot_type similar to the part_type

    public bool slotComponent;

    public string slotType;

    public string partType;

    public string partName;
    public string partDescription;

    private GameObject targetLocation;
    private Vector3 hold_position;
    private PhotonView currentPhotonView;

    [System.NonSerialized] public bool attached;
    [System.NonSerialized] public int listIndex;

    [PunRPC]
    public void SyncPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    [PunRPC]
    public void SyncBotColor(float r, float g, float b, float a)
    {
        var colorVec = new Vector4(r,g,b,a);
        GetComponent<SpriteRenderer>().color = colorVec;
    }

    [PunRPC]
    public void SyncIsEnemy(bool isEnemy)
    {
        ENEMY = isEnemy;

        var statBlock = GetComponent<StandardStatBlock>();
        if (statBlock != null)
        {
            statBlock.ENEMY = ENEMY;
        }
    }

    // Valid part Types include
    // LEG 
    // HEAD
    // ARM
    // ARMOR 
    // TORSO
    // TOPLARGE - CHANGED TO MOD
    // TOPSMALL - NO LONGER USED

    // Start is called before the first frame update
    void Start()
    {
        // sets part and slot types accordingly if the gameobject is not a part or not a slot
        // respectively

        if (slotComponent)
        {
            partType = "NOT A PART";
        }
        else if (!slotComponent)
        {
            slotType = "NOT A SLOT";
        }

        // checks for validity and correct spelling of slot and part types

        if (slotComponent)
        {
            if (slotType != "LEG" && slotType != "HEAD" && slotType != "ARM" && slotType != "MOD" && slotType != "ARMOR" && slotType != "TORSO")
            {
                Debug.Log(gameObject.transform.parent.gameObject.name + " " + slotType);
                Debug.Log("!!!INVALID SLOT TYPE!!!");
            }
        }
        else if (!slotComponent)
        {
            if (partType != "LEG" && partType != "HEAD" && partType != "ARM" && partType != "MOD" && partType != "ARMOR" && partType != "TORSO")
            {
                Debug.Log(gameObject.name);
                Debug.Log("!!!INVALID PART TYPE!!!");
            }
        }

        currentPhotonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!slotComponent)
        {
            if (currentPhotonView != null)
            {
                if (currentPhotonView.IsMine)
                { 
                    if (hold_position.x != transform.position.x || hold_position.y != transform.position.y || hold_position.z != transform.position.z)
                    {
                        hold_position = transform.position;
                        currentPhotonView.RPC("SyncPosition", RpcTarget.Others, hold_position.x, hold_position.y, hold_position.z);
                    }
                }
            }
        }
    }

    // function called when part is attached. If the part is not a LEG part then
    // finds the double parent since all parts that can have parts attached to them will
    // contain an SSB. Finds the SSB and transmits stats. Also adds a single count to 
    // melee component (any value greater than 1 sets the bot to melee enabled)

    public void add_stats()
    {
        if (partType != "LEG")
        {
            targetLocation = gameObject.transform.parent.transform.parent.gameObject;
        }
        else if (partType == "LEG")
        {
            targetLocation = gameObject;
        }
        targetLocation.GetComponent<StandardStatBlock>().transmitStats(PLATE, LOGIC, RANGE, ARMOR, SPEED, FUEL, POWER, COST);

        if (meleeComponent)
        {
            targetLocation.GetComponent<StandardStatBlock>().transmit_melee(1);
        }
    }

    // similar to add stats only values are negative.

    public void remove_stats()
    {
        if (partType != "LEG")
        {
            targetLocation = gameObject.transform.parent.transform.parent.gameObject;           
        }

        targetLocation.GetComponent<StandardStatBlock>().transmitStats(-PLATE, -LOGIC, -RANGE, -ARMOR, -SPEED, -FUEL, -POWER, -COST);

        if (meleeComponent)
        {
            targetLocation.GetComponent<StandardStatBlock>().transmit_melee(-1);
        }
    }

    public void deconstruct()
    {
        int childCount = transform.childCount;
        for (int childIndex = 0; childIndex < childCount; childIndex++)
        {
            var part = transform.GetChild(childIndex).gameObject.GetComponent<PartStats>();
            if (part != null)
            {
                part.deconstruct();
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
