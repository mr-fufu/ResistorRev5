using Photon.Pun;
using UnityEngine;

public class SpawnController : MonoBehaviourPunCallbacks
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

    public GameObject[] spawnBots;
    public int NUM_BOTS = 4;

    public GameObject select_bar;

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    public GameObject enemy_spawn_1;
    public GameObject enemy_spawn_2;
    public GameObject enemy_spawn_3;

    public GameObject spawn_buttons;
    public GameObject enemy_spawn_buttons;

    private GameObject selected_spawn_bot;

    public GameObject[] spawn_lane;

    public string bot_tag;

    private int select_val;

    public string player_name;

    public SpawnList spawn_list;
    private int credit_cost;

    void Start()
    {
        // Search for the 4 bots built in the workshop
        spawnBots = new GameObject[NUM_BOTS];
        for(int i = 1; i < 5; i++)
        {
            var curSpawnBot = GameObject.Find("StageArea" + i);
            if(curSpawnBot != null)
            {
                curSpawnBot.transform.localScale = new Vector3(1, 1, 1);
                spawnBots[i - 1] = curSpawnBot;
            }
        }

        // IMPORTANT! This section below is confusing but vital. It separates what is shown based on
        // whether the current instance of the game is the client or the server. 

        if (PhotonNetwork.IsMasterClient)
        {
            //If the instance is the server then we will refer to that instance as player 1

            // the credits cost for building bots will be deducted from credits1 (on the left side of the screen)
            credit_values = credits1;
            player_name = "Player1";

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
        else
        {
            //If the instance is not the server then we will refer to that instance as player 2

            // the credits cost for building bots will be deducted from credits2 (on the right side of the screen)

            credit_values = credits2;
            player_name = "Player2";

            // The spawn lanes will be enemy_spawn1, enemy_spawn2, and enemy_spawn3 (on the right side of the screen)
            spawn_lane = new GameObject[3];
            spawn_lane[0] = enemy_spawn_1;
            spawn_lane[1] = enemy_spawn_2;
            spawn_lane[2] = enemy_spawn_3;

            // Only the enemy_spawn_buttons will be shown (on the right side of the screen)
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

        // convert that int into a gameobject
        selected_spawn_bot = spawnBots[select_val-1];

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

    public void SpawnLane(int laneIndex)
    {
        if (!spawn_lane[laneIndex - 1].GetComponent<SpawnBlocker>().spawn_blocked)
        {
            if (selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST <= credit_values.GetComponent<CreditCounter>().credit_value)
            {
                credit_cost = selected_spawn_bot.transform.GetChild(0).GetChild(0).GetComponent<StandardStatBlock>().COST;

                player.GetComponent<PlayerSpawnScript>().SpawnBot(spawn_list.part_type_list, spawn_list.name_list, spawn_list.parent_count_list, spawn_list.child_count_1_list, spawn_list.child_count_2_list, spawn_list.child_count_3_list, spawn_lane[laneIndex - 1], "LANE" + laneIndex, credit_values, credit_cost);
            }
        }
    }
}