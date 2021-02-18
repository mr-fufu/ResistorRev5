using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawnScript : MonoBehaviourPunCallbacks, IPunObservable
{
    // So, basically: the player gameobject that the network lobby manager spawns is the only gameobject that can
    // spawn prefabs into the scene. Specifically, only scripts attached to the prefab. Thus, since the player is spawned
    // and not a part of the scene it would be a lot of work to find all the gameobjects by name. Thus, the solution
    // used here is to have other scripts that need to spawn prefabs find the player gameobject and pass all the information
    // including the gameobject to spawn, location to spawn, parent, and other parameters to the PlayerSpawnScript to
    // spawn the gameobject instead.

    private GameObject base_component;
    public GameObject colorPrefab;

    public bool player_1;
    public string bot_tag;

    [SerializeField] private GameObject partsLibraryObject;
    public PartLibrary[] part_library;
    private int part_type_index;

    public Vector4 network_color;
    public Vector4 bot_color;

    /* Synced values */
    public float player_red;
    public float player_blue;
    public float player_green;

    private int part_count;

    public Transform slot_position;

    private Vector3 color_object_location;

    void Start()
    {
        // search for the network manager and if found, get the color selected in the lobby scene and use it to spawn bots of the chosen color

        // Initialize the part library. Essentially the part library is an array of part_library components
        part_library = new PartLibrary[6];

        for (int i = 0; i < 6; i++)
        {
            part_library[i] = partsLibraryObject.transform.GetChild(i).GetComponent<PartLibrary>();
        }

        // proper sorting of the player. Since there are 2 instances, both with 2 players, there are 4 different
        // possible configurations for the player. On the server side the player with authority is player 1 and on the left
        // side of the screen while the player without authority is player 2 on the right. On the non-server client the
        // player that has authority is player 2 and the player without is player 1.

        if (PhotonNetwork.IsMasterClient)
        {
            gameObject.name = "Player1";
            player_1 = true;
            bot_tag = "BOT_Player";
        }
        else
        {
            gameObject.name = "Player2";
            player_1 = false;
            bot_tag = "BOT_Enemy";
        }

        SpawnColorObject();

        player_red = network_color.x;
        player_blue = network_color.y;
        player_green = network_color.z;

        bot_color = new Vector4(player_red, player_blue, player_green, 1);
    }

    // Bot spawning command function. Takes the spawn list arrays as well as the spawn location, name of the lane the bot is spawned into, the credits object
    // to be deducted from, and the amount of credits to deduct for spawning the bot.

    public void SpawnBot(string[] part_type_list, string[] name_list, int[] parent_count_list, int[] child_count_1_list, int[] child_count_2_list, int[] child_count_3_list, GameObject spawn_location, string lane_name, GameObject credit_object, int credit_cost)
    {
        // Instantiate the leg of the bot first
        //------------------------------------------------------------------

        // find the correct part type list in order to find the index the correct part library
        // In this case, it will always be the LEG part library for leg part
        part_type_index = library_index(part_type_list[0]);

        // instantiate leg part (0th element in all spawn list arrays)

        GameObject bot_clone = (GameObject) PhotonNetwork.Instantiate("PartPrefabs/LEGprefabs/" + part_library[part_type_index].part_library[search_library(name_list[0], part_type_index)].name, spawn_location.transform.position, spawn_location.transform.rotation);
        
        // deduct credits for spawning bot
        credit_object.GetComponent<CreditCounter>().credit_value -= credit_cost;
        //credit_object.GetComponent<PhotonView>().RPC("SyncCredits", RpcTarget.Others, credit_cost);

        // set the newly spawned bot's automove script's spawned bool to true. Set the sorting layer to the one passed through lane_name
        
        bot_clone.GetComponent<PhotonView>().RPC("SyncIsEnemy", RpcTarget.All, !PhotonNetwork.IsMasterClient);
        bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
 
        // set the ENEMY of the SSB to the appropriate value depending on the player_1 bool (true for player 1 and false for player 2
        // i.e. player 2 controls enemy bots on the right side of screen. Similarly, set the bot_tag to the appropriate tag (BOT_Player or
        // BOT_Enemy)

        bot_clone.gameObject.tag = bot_tag;

        // unfortunately, the layer issue has been slightly more complicated to solve. Ideally, set the bot part gameobjects
        // to the correct sorting layer based on lane (different from sprite sorting layer). Currently does not work for
        // unknown reason and was changed to put all bots on layer 13. Ideally, Automove script searches for bots only on the
        // same lane sorting layer.
        bot_clone.gameObject.layer = 13;

        // enable the collider for the bot and adjust its size if the bot uses melee attacks. Melee collider is slightly smaller
        // to allow the bot to move in closer.
        bot_clone.GetComponent<Collider2D>().enabled = true;

        if (bot_clone.GetComponent<StandardStatBlock>().melee_components > 0)
        {
            bot_clone.GetComponent<Collider2D>().offset = new Vector2(-5, 25);
            bot_clone.GetComponent<BoxCollider2D>().size = new Vector2(25, 50);
        }
        else
        {
            bot_clone.GetComponent<Collider2D>().offset = new Vector2(6, 25);
            bot_clone.GetComponent<BoxCollider2D>().size = new Vector2(40, 50);
        }

        //--------------------------------------------------------------------------
        // Adjust Bot stats

        // add the stats from the part stats script to the SSB
        bot_clone.GetComponent<PartStats>().add_stats();

        var statBlock = bot_clone.GetComponent<StandardStatBlock>();
        if (statBlock != null)
        {
            statBlock.UpdateStats();
        }
        
        if (bot_clone.GetComponent<PartStats>().part_type == "ARM")
        {
            bot_clone.transform.position = new Vector3(bot_clone.transform.position.x, bot_clone.transform.position.y, -bot_clone.transform.position.y * 0.1f - 10);
        }

        bot_clone.GetComponent<PartStats>().attached = true;

        //--------------------------------------------------------------------------

        ChangeChildren(bot_clone, bot_color, lane_name);

        base_component = bot_clone;


        //--------------------------------------------------------------------
        // Instantiate Parts

        for (int index2 = 1; index2 < name_list.Length; index2++)
        {
            part_type_index = library_index(part_type_list[index2]);

            // Photon networking requires the folder path under Assets/Resources when instantiating
            var partPrefab = part_library[part_type_index].part_library[search_library(name_list[index2], part_type_index)];
            var partType = partPrefab.GetComponent<PartStats>().part_type;
            if(partType.Equals("TOPLARGE"))
            {
                partType = "TOP";
            }
            
            string path = "PartPrefabs/" + partType + "Prefabs/" + partPrefab.name;

            GameObject part_clone = PhotonNetwork.Instantiate(path, spawn_location.transform.position, spawn_location.transform.rotation);

            part_clone.gameObject.tag = bot_tag;

            if (parent_count_list[index2] == 1)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).transform;
            }
            else if (parent_count_list[index2] == 2)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).GetChild(0).GetChild(child_count_2_list[index2]).transform;
            }
            else if (parent_count_list[index2] == 3)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).GetChild(0).GetChild(child_count_2_list[index2]).GetChild(0).GetChild(child_count_3_list[index2]).transform;
            }

            part_clone.transform.parent = slot_position;
            part_clone.transform.localPosition = new Vector2(0, 0);
            part_clone.transform.localScale = new Vector3(1, 1, 1);

            part_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;

            part_clone.gameObject.layer = 13;

            //--------------------------------------------------------------------------
            // Adjust Bot stats

            ChangeChildren(part_clone, bot_color, lane_name);
            part_clone.GetComponent<PartStats>().add_stats();

            if (part_clone.GetComponent<PartStats>().part_type == "ARM")
            {
                part_clone.transform.position = new Vector3(part_clone.transform.position.x, part_clone.transform.position.y, -part_clone.transform.position.y * 0.1f - 10);
                part_clone.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 8;
            }
            else
            {
                part_clone.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
            }

            part_clone.GetComponent<PartStats>().attached = true;
        }
    }

    // change children function. Find all child gameobjects that are part slots and sets all
    // children of part slots that are parts to the input change_color and lane_name. If part changed
    // has any children, nest change_children

    public void ChangeChildren(GameObject selected_object, Color change_color, string lane_name)
    {
        part_count = selected_object.transform.childCount;

        if (part_count != 0)
        {
            for (int var = 0; var < (part_count); var++)
            {
                var selectObjChild = selected_object.transform.GetChild(var);

                if (selectObjChild.GetComponent<PartStats>() != null)
                {
                    if (selectObjChild.GetComponent<PartStats>().slot_component)
                    {
                        if (selectObjChild.childCount != 0)
                        {
                            int slot_child_count = selected_object.transform.GetChild(var).childCount;

                            for (int slot_index = 0; slot_index < slot_child_count; slot_index++)
                            {
                                if (selectObjChild.GetChild(slot_index).GetComponent<PartStats>() != null)
                                {

                                    selectObjChild.GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                    selectObjChild.GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);

                                    if (selectObjChild.GetChild(slot_index).childCount != 0)
                                    {
                                        ChangeChildren(selectObjChild.GetChild(slot_index).gameObject, new Vector4(1, 1, 1, 1), lane_name);

                                        part_count = selected_object.transform.childCount;
                                        slot_child_count = selectObjChild.childCount;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SpawnColorObject()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        color_object_location = player_1 ? new Vector3(-300, 200, 0) : new Vector3(300, 200, 0);

        var botColorObj = PhotonNetwork.Instantiate(colorPrefab.name, color_object_location, Quaternion.Euler(Vector3.zero));


        //TODO SAM: NOT RANDOM
        botColorObj.GetComponent<BotColorObject>().UpdateColors(Random.Range(0,4), true);
    }

    // Given a part_name and an index, search the appropriate library of the 
    // associated type until a matching name is found and return the index of the part
    // in that part library.

    public int search_library(string part_name, int part_type)
    {

        for (int search_index = 0; search_index < part_library[part_type].part_library.Length; search_index++)
        {
            if (part_library[part_type].part_library[search_index].GetComponent<PartStats>().part_name == part_name)
            {
                return search_index;
            }
        }

        return 0;
    }

    // Quick check for converting part_type to an index in the
    // part library (in order to find the indexed library corresponding
    // to the searched part). e.g. ARM part will return the index
    // of the ARM part library (index 1)

    public int library_index(string part_type)
    {
        if (part_type == "LEG")
        {
            return 0;
        }
        else if (part_type == "ARM")
        {
            return 1;
        }
        else if (part_type == "TORSO")
        {
            return 2;
        }
        else if (part_type == "HEAD")
        {
            return 3;
        }
        else if (part_type == "TOPLARGE")
        {
            return 4;
        }
        else if (part_type == "ARMOR")
        {
            return 5;
        }
        else
        {
            return 6;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}

