using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SP_SpawnController : NetworkBehaviour
{
    // The SpawnController script manages the "showcase" bots (bots that were built in the Workshop Scene and are comprised of workshop_show_piece enabled bot parts)
    // Here, the bot colours, showcase bots, credits and spawn positions are managed. The select bar script is also linked in order to find what bot is currently
    // selected by the player. Buttons are attached to the spawn functions which then finds the currently selected bot, deducts the appropriate amount of credits, and
    // spawns the bot by passing the SpawnList information from the showcase bot as well as the lane to spawn in. Previously, the bot was spawned using this script alone
    // but when moved to multiplayer, only the player object had the authority to spawn prefabs. Leftover code has been commented out.

    public GameObject credits1;
    public GameObject credits2;
    public GameObject credit_values;

    public GameObject player;

    //-------------------------------------------------------------------------

    //public GameObject leg_library;
    //public GameObject arm_library;
    //public GameObject torso_library;
    //public GameObject head_library;
    //public GameObject top_library;
    //public GameObject armor_library;

    //private PartLibrary[] part_library;

    //private int part_type_index;

    //-------------------------------------------------------------------------

    public GameObject spawn_bot_1;
    public GameObject spawn_bot_2;
    public GameObject spawn_bot_3;
    public GameObject spawn_bot_4;

    public GameObject select_bar;

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    //-------------------------------------------------------------------------

    //private Transform slot_position;

    //private GameObject base_component;

    //private GameObject[] contained_parts;
    //public int part_count;

    //private Color[] bot_color;

    //-------------------------------------------------------------------------

    public GameObject enemy_spawn_1;
    public GameObject enemy_spawn_2;
    public GameObject enemy_spawn_3;

    public GameObject spawn_buttons;
    public GameObject enemy_spawn_buttons;

    private GameObject selected_spawn_bot;

    public GameObject[] spawn_lane;
    private string[] lane_name;

    public string bot_tag;

    private int select_val;

    //public bool player_1;
    //public int player_no;
    public string player_name;

    public SpawnList spawn_list;
    private int credit_cost;

    void Start()
    {
        /*
        This section of code was part of the spawn function which has been moved to the PlayerSpawnScript. More
        details are provided in the actual commented out function.

        part_library = new PartLibrary[6];
        part_library[0] = leg_library.GetComponent<PartLibrary>();
        part_library[1] = arm_library.GetComponent<PartLibrary>();
        part_library[2] = torso_library.GetComponent<PartLibrary>();
        part_library[3] = head_library.GetComponent<PartLibrary>();
        part_library[4] = top_library.GetComponent<PartLibrary>();
        part_library[5] = armor_library.GetComponent<PartLibrary>();

        bot_color = new Color[2];
        bot_color[0] = new Vector4(1, 1, 1, 1);
        bot_color[1] = new Vector4(1, 0.75f, 0.75f, 1);
        */

        //-------------------------------------------------------------------------

        // Search for the 4 bots built in the workshop

        spawn_bot_1 = GameObject.Find("StageArea1");
        spawn_bot_2 = GameObject.Find("StageArea2");
        spawn_bot_3 = GameObject.Find("StageArea3");
        spawn_bot_4 = GameObject.Find("StageArea4");

        // if all 4 robots were built (always true unless the workshop bypass button for testing purposes is used)
        // then change their local scale to 1

        if (spawn_bot_1 != null)
        {
            spawn_bot_1.transform.localScale = new Vector3(1, 1, 1);
        }

        if (spawn_bot_2 != null)
        {
            spawn_bot_2.transform.localScale = new Vector3(1, 1, 1);
        }

        if (spawn_bot_3 != null)
        {
            spawn_bot_3.transform.localScale = new Vector3(1, 1, 1);
        }

        if (spawn_bot_4 != null)
        {
            spawn_bot_4.transform.localScale = new Vector3(1, 1, 1);
        }

        //-------------------------------------------------------------------------

        // lane_name is just a string that represents which lane the bot will spawn in
        // a string is used instead of an integer since the string will be assigned to the sorting layer by name
        lane_name = new string[3];

        lane_name[0] = "LANE1";
        lane_name[1] = "LANE2";
        lane_name[2] = "LANE3";

        //-------------------------------------------------------------------------

        // IMPORTANT! This section below is confusing but vital. It separates what is shown based on
        // whether the current instance of the game is the client or the server. 

        if (isServer)
        {
            //If the instance is the server then we will refer to that instance as player 1

            // the credits cost for building bots will be deducted from credits1 (on the left side of the screen)
            credit_values = credits1;

            // We will refer to this instance by a string ("Player1"), a bool (true) NOT USED and an int (0) NOT USED
            player_name = "Player1";
            //player_1 = true;
            //player_no = 0;

            // The spawn lanes will be spawn1, spawn2, and spawn3 (on the left side of the screen)
            spawn_lane = new GameObject[3];

            spawn_lane[0] = spawn1;
            spawn_lane[1] = spawn2;
            spawn_lane[2] = spawn3;

            // Only the spwan_buttons will be shown (on the left side of the screen)
            spawn_buttons.SetActive(true);
            enemy_spawn_buttons.SetActive(false);

            // Bots built will have the tag "BOT_Player"
            bot_tag = "BOT_Player";
        }
        else if (!isServer)
        {
            //If the instance is not the server then we will refer to that instance as player 2

            // the credits cost for building bots will be deducted from credits2 (on the right side of the screen)

            credit_values = credits2;

            // We will refer to this instance by a string ("Player2"), a bool (false) NOT USED and an int (1) NOT USED

            player_name = "Player2";
            //player_1 = false;
            //player_no = 1;

            // The spawn lanes will be enemy_spawn1, enemy_spawn2, and enemy_spawn3 (on the right side of the screen)
            spawn_lane = new GameObject[3];

            spawn_lane[0] = enemy_spawn_1;
            spawn_lane[1] = enemy_spawn_2;
            spawn_lane[2] = enemy_spawn_3;

            // Only the enemy_spwan_buttons will be shown (on the right side of the screen)
            spawn_buttons.SetActive(false);
            enemy_spawn_buttons.SetActive(true);

            // Bots built will have the tag "BOT_Enemy"
            bot_tag = "BOT_Enemy";
        }
    }

    private void Update()
    {
        // find the player gameobject to pass spawn info
        if (player == null)
        {
            player = GameObject.Find(player_name);
        }
        // pull the selected value from the select bar gameobject which tells us which bot out of the 4 built
        // is currently selected by the player
        select_val = select_bar.GetComponent<BarSelectParentLink>().selected_box;

        // convert that int into a gameobject. Could be done also by using an index with an array of gameObjects, makes no difference
        if (select_val == 1)
        {
            selected_spawn_bot = spawn_bot_1;
        }
        else if (select_val == 2)
        {
            selected_spawn_bot = spawn_bot_2;
        }
        else if (select_val == 3)
        {
            selected_spawn_bot = spawn_bot_3;
        }
        else if (select_val == 4)
        {
            selected_spawn_bot = spawn_bot_4;
        }

        // the spawn_list is then the spawn list of the selected bot
        if (selected_spawn_bot != null)
        {
            spawn_list = selected_spawn_bot.GetComponent<SpawnList>();
        }
    }

    // These three functions below are all functionally identical save for which lane the bot will spawn in. The functions are called by 3 buttons, each associated
    // with a single lane. The spawn function first checks that the spawn is not blocked by another bot which has not yet left the spawn area (this is to avoid a bug where
    // bots spawned too close together and were collided with one another), then checks that there are an adequate number of credits to build that bot and if there are, the function
    // then passes to the PlayerSpawnScript attached to the appropriate player the spawn_list information (since the spawn list function itself could not be passed for whatever reason)
    // as well as the lane the bot is to be spawned in, the cost of the bot, and the credits object where the cost will be deducted from.

    public void SpawnLane1()
    {
        if (!spawn_lane[0].GetComponent<SpawnBlocker>().spawn_blocked)
        {

            //player.GetComponent<PlayerSpawnScript>().part_library = part_library;

            if (selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST <= credit_values.GetComponent<CreditCounter>().credit_value)
            {
                //credit_values.GetComponent<CreditCounter>().credit_value -= selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;
                credit_cost = selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;

                player.GetComponent<PlayerSpawnScript>().CmdSpawnBot(spawn_list.part_type_list, spawn_list.name_list, spawn_list.parent_count_list, spawn_list.child_count_1_list, spawn_list.child_count_2_list, spawn_list.child_count_3_list, spawn_lane[0], "LANE1", credit_values, credit_cost);
            }

            //CmdSpawnBot(selected_spawn_bot, 0, player_1);
        }
    }

    public void SpawnLane2()
    {
        if (!spawn_lane[1].GetComponent<SpawnBlocker>().spawn_blocked)
        {

            //player.GetComponent<PlayerSpawnScript>().part_library = part_library;

            if (selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST <= credit_values.GetComponent<CreditCounter>().credit_value)
            {
                //credit_values.GetComponent<CreditCounter>().credit_value -= selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;
                credit_cost = selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;

                player.GetComponent<PlayerSpawnScript>().CmdSpawnBot(spawn_list.part_type_list, spawn_list.name_list, spawn_list.parent_count_list, spawn_list.child_count_1_list, spawn_list.child_count_2_list, spawn_list.child_count_3_list, spawn_lane[1], "LANE2", credit_values, credit_cost);
            }

            //CmdSpawnBot(selected_spawn_bot, 1, player_1);
        }
    }

    public void SpawnLane3()
    {
        if (!spawn_lane[2].GetComponent<SpawnBlocker>().spawn_blocked)
        {

            //player.GetComponent<PlayerSpawnScript>().part_library = part_library;

            if (selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST <= credit_values.GetComponent<CreditCounter>().credit_value)
            {
                //credit_values.GetComponent<CreditCounter>().credit_value -= selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;
                credit_cost = selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;

                player.GetComponent<PlayerSpawnScript>().CmdSpawnBot(spawn_list.part_type_list, spawn_list.name_list, spawn_list.parent_count_list, spawn_list.child_count_1_list, spawn_list.child_count_2_list, spawn_list.child_count_3_list, spawn_lane[2], "LANE3", credit_values, credit_cost);
            }

            //CmdSpawnBot(selected_spawn_bot, 2, player_1);
        }
    }

    /*
     
    The command for spawning bots has been moved to the script PlayerSpawnScript. Bots can't be spawned from this script since
    the player gameobject is the only gameobject that has the authority to spawn prefabs into the scene.

    //-------------------------------------------------------------------------

    [Command]
    public void CmdSpawnBot(GameObject spawn_bot, int lane_number, bool player_1)
    {
        if (player_1)
        {
            player_no = 0;
        }
        else
        {
            player_no = 1;
        }

        if (spawn_lane[lane_number].GetComponent<SpawnBlocker>().spawn_blocked != true)
        {
            if (spawn_bot.transform.GetChild(0).transform.childCount != 0)
            {
                if (selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST <= credit_values[0].GetComponent<CreditCounter>().credit_value)
                {
                    credit_values[player_no].GetComponent<CreditCounter>().credit_value -= selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;

                    //spawn_list.name_list.Length

                    //-------------------------------------------------------------------- 
                    // Instantiate Legs

                    for (int index = 0; index < 1; index++)
                    {
                        part_type_index = library_index(spawn_list.part_type_list[index]);

                        var bot_clone = (GameObject)Instantiate(part_library[part_type_index].part_library[search_library(spawn_list.name_list[index], part_type_index)], spawn_lane[lane_number].position, spawn_lane[lane_number].rotation);

                        bot_clone.GetComponent<AutoMove>().spawned = true;
                        bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

                        if (player_1 == true)
                        {
                            bot_clone.GetComponent<StandardStatBlock>().ENEMY = true;
                        }
                        else
                        {
                            bot_clone.GetComponent<StandardStatBlock>().ENEMY = false;
                        }

                        bot_clone.gameObject.tag = bot_tag[player_no];
                        bot_clone.gameObject.layer = 13;

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
                        //Copied from Workshop Drag


                        bot_clone.GetComponent<PartStats>().add_stats();

                        //bot_clone.transform.localScale = placement.transform.localScale;

                        if (bot_clone.GetComponent<PartStats>().part_type == "ARM")
                        {
                            bot_clone.transform.position = new Vector3(bot_clone.transform.position.x, bot_clone.transform.position.y, -bot_clone.transform.position.y * 0.1f - 10);
                        }

                        bot_clone.GetComponent<PartStats>().attached = true;
                        //slot_component.gameObject.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;


                        //--------------------------------------------------------------------------

                        change_children(bot_clone, bot_color[player_no], lane_name[lane_number]);

                        //bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

                        NetworkServer.Spawn(bot_clone);



                        base_component = bot_clone;
                    }

                    //--------------------------------------------------------------------
                    // Instantiate Parts

                    for (int index2 = 1; index2 < spawn_list.name_list.Length; index2++)
                    {
                        part_type_index = library_index(spawn_list.part_type_list[index2]);

                        var part_clone = (GameObject)Instantiate(part_library[part_type_index].part_library[search_library(spawn_list.name_list[index2], part_type_index)], spawn_lane[lane_number].position, spawn_lane[lane_number].rotation);

                        if (spawn_list.parent_count_list[index2] == 1)
                        {
                            slot_position = base_component.transform.GetChild(0).transform;

                            part_clone.transform.parent = slot_position;
                            part_clone.transform.localPosition = new Vector2(0,0);
                            part_clone.transform.localScale = new Vector3(1, 1, 1);
                        }
                        else if (spawn_list.parent_count_list[index2] == 2)
                        {
                            slot_position = base_component.transform.GetChild(0).GetChild(0).GetChild(spawn_list.child_count_1_list[index2]).transform;

                            part_clone.transform.parent = slot_position;
                            part_clone.transform.localPosition = new Vector2(0, 0);
                            part_clone.transform.localScale = new Vector3(1, 1, 1);
                        }
                        else if (spawn_list.parent_count_list[index2] == 3)
                        {
                            slot_position = base_component.transform.GetChild(0).GetChild(0).GetChild(spawn_list.child_count_1_list[index2]).GetChild(0).GetChild(spawn_list.child_count_2_list[index2]).transform;

                            part_clone.transform.parent = slot_position;
                            part_clone.transform.localPosition = new Vector2(0, 0);
                            part_clone.transform.localScale = new Vector3(1, 1, 1);
                        }
                        else if (spawn_list.parent_count_list[index2] == 4)
                        {
                            slot_position = base_component.transform.GetChild(0).GetChild(0).GetChild(spawn_list.child_count_1_list[index2]).GetChild(0).GetChild(spawn_list.child_count_2_list[index2]).GetChild(0).GetChild(spawn_list.child_count_3_list[index2]).transform;

                            part_clone.transform.parent = slot_position;
                            part_clone.transform.localPosition = new Vector2(0, 0);
                            part_clone.transform.localScale = new Vector3(1, 1 ,1);
                        }

                        part_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

                        part_clone.gameObject.tag = bot_tag[player_no];
                        part_clone.gameObject.layer = 13;

                        //--------------------------------------------------------------------------
                        //Copied from Workshop Drag

                        part_clone.GetComponent<PartStats>().add_stats();

                        //bot_clone.transform.localScale = placement.transform.localScale;

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

                        //slot_component.gameObject.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;
                        //--------------------------------------------------------------------------

                        change_children(part_clone, bot_color[player_no], lane_name[lane_number]);

                        //part_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

                        NetworkServer.Spawn(part_clone);


                    }

                }
            }
        }

        //var bot_clone = (GameObject)Instantiate(selected_spawn_bot.transform.GetChild(0).GetChild(0).gameObject, spawn_lane[lane_number].position, spawn_lane[lane_number].rotation);
        //bot_clone.GetComponent<AutoMove>().spawned = true;
        //bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];
        //bot_clone.gameObject.tag = bot_tag[player_number];
        //bot_clone.gameObject.layer = 13;
        //bot_clone.GetComponent<Collider2D>().enabled = true;

        

        if (player_number == 1)
        {
            bot_clone.GetComponent<StandardStatBlock>().ENEMY = true;
        }

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

        change_children(bot_clone, bot_color[lane_number], lane_name[lane_number]);

        

        //NetworkServer.Spawn(bot_clone);
        //        }
        //    }
        //}
    }
    

    public void change_children(GameObject selected_object, Color change_color, string lane_name)
    {
        part_count = selected_object.transform.childCount;

        if (part_count != 0)
        {
            for (int var = 0; var < (part_count); var++)
            {
                if (selected_object.transform.GetChild(var).GetComponent<PartStats>() != null)
                {
                    if (selected_object.transform.GetChild(var).GetComponent<PartStats>().slot_component)
                    {
                        if (selected_object.transform.GetChild(var).childCount != 0)
                        {

                            if (selected_object.transform.GetChild(var).GetChild(0).gameObject.transform.childCount != 0)
                            {
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = change_color;
                                change_children(selected_object.transform.GetChild(var).GetChild(0).gameObject, change_color, lane_name);
                                part_count = selected_object.transform.childCount;
                            }
                            else
                            {
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = change_color;
                            }
                        }
                    }
                }
            }
        }
    }

    public int search_library(string part_name, int part_type)
    {

        for (int search_index = 0; search_index < part_library[part_type].part_library.Length; search_index++)
        {
            if (part_library[part_type].part_library[search_index].GetComponent<PartStats>().part_name == part_name)
            {
                //Debug.Log("Part Found!");
                //Debug.Log(part_name);
                //Debug.Log(part_library[part_type].part_library[search_index].name);
                //Debug.Log(part_type);
                //Debug.Log(search_index);

                return search_index;
            }
        }

        return 0;
    }

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
        else if (part_type == "TOP")
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
    */
}
