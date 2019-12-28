using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SP_PlayerSpawnScript : NetworkBehaviour
{
    // So, basically: the player gameobject that the network lobby manager spawns is the only gameobject that can
    // spawn prefabs into the scene. Specifically, only scripts attached to the prefab. Thus, since the player is spawned
    // and not a part of the scene it would be a lot of work to find all the gameobjects by name. Thus, the solution
    // used here is to have other scripts that need to spawn prefabs find the player gameobject and pass all the information
    // including the gameobject to spawn, location to spawn, parent, and other parameters to the PlayerSpawnScript to
    // spawn the gameobject instead.

    private GameObject base_component;
    private GameObject network_manager;
    public GameObject color_object;

    public bool player_1;
    public string bot_tag;

    public PartLibrary[] part_library;
    private int part_type_index;

    public Vector4 network_color;
    public Vector4 bot_color;

    [SyncVar] public float player_red;
    [SyncVar] public float player_blue;
    [SyncVar] public float player_green;

    private int part_count;

    public Transform slot_position;

    private float lightning_dist;
    private Vector3 lightning_point;
    private bool alternator;

    private Vector3 color_object_location;

    void Start()
    {
        // search for the network manager and if found, get the color selected in the lobby scene and use it to spawn bots of the chosen color

        // Initialize the part library. Essentially the part library is an array of part_library components
        part_library = new PartLibrary[6];
        part_library[0] = transform.GetChild(0).GetChild(0).GetComponent<PartLibrary>();
        part_library[1] = transform.GetChild(0).GetChild(1).GetComponent<PartLibrary>();
        part_library[2] = transform.GetChild(0).GetChild(2).GetComponent<PartLibrary>();
        part_library[3] = transform.GetChild(0).GetChild(3).GetComponent<PartLibrary>();
        part_library[4] = transform.GetChild(0).GetChild(4).GetComponent<PartLibrary>();
        part_library[5] = transform.GetChild(0).GetChild(5).GetComponent<PartLibrary>();

        // proper sorting of the player. Since there are 2 instances, both with 2 players, there are 4 different
        // possible configurations for the player. On the server side the player with authority is player 1 and on the left
        // side of the screen while the player without authority is player 2 on the right. On the non-server client the
        // player that has authority is player 2 and the player without is player 1.

        if (isServer && hasAuthority)
        {
            gameObject.name = "Player1";
            player_1 = true;
            bot_tag = "BOT_Player";

            network_manager = GameObject.Find("NetworkManager");

            CmdSpawnColorObject(network_manager.GetComponent<CustomNetworkManager>().port_alt, network_manager.GetComponent<CustomNetworkManager>().port_no, false);
        }
        else if (!isServer && hasAuthority)
        {
            gameObject.name = "Player2";
            player_1 = false;
            bot_tag = "BOT_Enemy";

            network_manager = GameObject.Find("NetworkManager");

            CmdSpawnColorObject(network_manager.GetComponent<CustomNetworkManager>().port_alt, network_manager.GetComponent<CustomNetworkManager>().port_no, true);
        }
        else if (isServer && !hasAuthority)
        {
            gameObject.name = "Player2";
            player_1 = false;
            bot_tag = "BOT_Enemy";
        }
        else if (!isServer && !hasAuthority)
        {
            gameObject.name = "Player1";
            player_1 = true;
            bot_tag = "BOT_Player";
        }

        player_red = network_color.x;
        player_blue = network_color.y;
        player_green = network_color.z;

        bot_color = new Vector4(player_red, player_blue, player_green, 1);
    }

    void Update()
    {

        // simply destroys the player object if the player doesn't have authority. This is
        // because we want to only instantiate a single bot and then update it onto the other
        // player's client. Therefore, you only ever spawn your own bots regardless of whether
        // the instance is the server or not and never instantiate bots on the side that is not 
        // yours. (i.e. player 1's client never instantiates player 2's bots and vice versa)
        if (!isLocalPlayer)
        {
            return;
        }

        if (!hasAuthority)
        {
            Destroy(gameObject);
        }

    }

    // Bot spawning command function. Takes the spawn list arrays as well as the spawn location, name of the lane the bot is spawned into, the credits object
    // to be deducted from, and the amount of credits to deduct for spawning the bot.

    [Command]
    public void CmdSpawnBot(string[] part_type_list, string[] name_list, int[] parent_count_list, int[] child_count_1_list, int[] child_count_2_list, int[] child_count_3_list, GameObject spawn_location, string lane_name, GameObject credit_object, int credit_cost)
    {
        // Instantiate the leg of the bot first
        //------------------------------------------------------------------

        // find the correct part type list in order to find the index the correct part library
        // In this case, it will always be the LEG part library for leg part
        part_type_index = library_index(part_type_list[0]);

        // instantiate leg part (0th element in all spawn list arrays)
        var bot_clone = (GameObject)Instantiate(part_library[part_type_index].part_library[search_library(name_list[0], part_type_index)], spawn_location.transform.position, spawn_location.transform.rotation);

        // deduct credits for spawning bot
        credit_object.GetComponent<CreditCounter>().credit_value -= credit_cost;

        // set the newly spawned bot's automove script's spawned bool to true. Set the sorting layer to the one passed through lane_name
        bot_clone.GetComponent<AutoMove>().spawned = true;
        bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;

        // set the ENEMY of the SSB to the appropriate value depending on the player_1 bool (true for player 1 and false for player 2
        // i.e. player 2 controls enemy bots on the right side of screen. Similarly, set the bot_tag to the appropriate tag (BOT_Player or
        // BOT_Enemy)
        if (player_1)
        {
            bot_clone.GetComponent<StandardStatBlock>().ENEMY = false;
        }
        else
        {
            bot_clone.GetComponent<StandardStatBlock>().ENEMY = true;
        }

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

        if (bot_clone.GetComponent<PartStats>().part_type == "ARM")
        {
            bot_clone.transform.position = new Vector3(bot_clone.transform.position.x, bot_clone.transform.position.y, -bot_clone.transform.position.y * 0.1f - 10);
        }

        bot_clone.GetComponent<PartStats>().attached = true;

        //slot_component.gameObject.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;

        //--------------------------------------------------------------------------

        ChangeChildren(bot_clone, bot_color, lane_name);

        //Debug.Log("Instantiated Bot : " + bot_clone.name);

        //bot_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

        NetworkServer.Spawn(bot_clone);

        base_component = bot_clone;


        //--------------------------------------------------------------------
        // Instantiate Parts

        for (int index2 = 1; index2 < name_list.Length; index2++)
        {
            part_type_index = library_index(part_type_list[index2]);

            var part_clone = (GameObject)Instantiate(part_library[part_type_index].part_library[search_library(name_list[index2], part_type_index)], spawn_location.transform.position, spawn_location.transform.rotation);

            part_clone.gameObject.tag = bot_tag;

            if (parent_count_list[index2] == 1)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).transform;

                part_clone.transform.parent = slot_position;
                part_clone.transform.localPosition = new Vector2(0, 0);
                part_clone.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (parent_count_list[index2] == 2)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).GetChild(0).GetChild(child_count_2_list[index2]).transform;

                part_clone.transform.parent = slot_position;
                part_clone.transform.localPosition = new Vector2(0, 0);
                part_clone.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (parent_count_list[index2] == 3)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).GetChild(0).GetChild(child_count_2_list[index2]).transform;

                part_clone.transform.parent = slot_position;
                part_clone.transform.localPosition = new Vector2(0, 0);
                part_clone.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (parent_count_list[index2] == 4)
            {
                slot_position = base_component.transform.GetChild(child_count_1_list[index2]).GetChild(0).GetChild(child_count_2_list[index2]).GetChild(0).GetChild(child_count_3_list[index2]).transform;

                part_clone.transform.parent = slot_position;
                part_clone.transform.localPosition = new Vector2(0, 0);
                part_clone.transform.localScale = new Vector3(1, 1, 1);
            }

            //Debug.Log("Parent Count = " + parent_count_list[index2]);

            //Debug.Log("Instantiated Part : " + part_clone.name + "In" + slot_position.name);

            part_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;

            part_clone.gameObject.layer = 13;

            //--------------------------------------------------------------------------
            // Adjust Bot stats

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

            ChangeChildren(part_clone, bot_color, lane_name);

            //part_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_number];

            NetworkServer.Spawn(part_clone);
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
                if (selected_object.transform.GetChild(var).GetComponent<PartStats>() != null)
                {
                    if (selected_object.transform.GetChild(var).GetComponent<PartStats>().slot_component)
                    {
                        if (selected_object.transform.GetChild(var).childCount != 0)
                        {
                            int slot_child_count = selected_object.transform.GetChild(var).childCount;

                            for (int slot_index = 0; slot_index < slot_child_count; slot_index++)
                            {
                                if (selected_object.transform.GetChild(var).GetChild(slot_index).GetComponent<PartStats>() != null)
                                {
                                    if (selected_object.transform.GetChild(var).GetChild(slot_index).childCount != 0)
                                    {
                                        selected_object.transform.GetChild(var).GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                        selected_object.transform.GetChild(var).GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1,1,1,1);

                                        ChangeChildren(selected_object.transform.GetChild(var).GetChild(slot_index).gameObject, new Vector4(1, 1, 1, 1), lane_name);

                                        part_count = selected_object.transform.childCount;
                                        slot_child_count = selected_object.transform.GetChild(var).childCount;
                                    }
                                    else
                                    {
                                        selected_object.transform.GetChild(var).GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                        selected_object.transform.GetChild(var).GetChild(slot_index).gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Spawn a Generic object. May be used later on for simple applications. Currently not in use (probably, check before
    // deleting)
    [Command]

    public void CmdSpawnColorObject(bool alt, int no, bool enemy)
    {
        if (player_1)
        {
            color_object_location = new Vector3(-300, 200, 0);
        }
        else
        {
            color_object_location = new Vector3(300, 200, 0);
        }

        var bot_color_object = (GameObject)Instantiate(color_object, color_object_location, Quaternion.Euler(Vector3.zero));

        bot_color_object.GetComponent<BotColorObject>().enemy = enemy;

        bot_color_object.GetComponent<BotColorObject>().alt = alt;
        bot_color_object.GetComponent<BotColorObject>().no = no;

        NetworkServer.Spawn(bot_color_object);
    }

    [Command]

    public void CmdSpawnGeneric(GameObject object_to_spawn, Vector2 spawn_location, GameObject parent_object, Quaternion object_rotation)
    {
        var spawned_object = (GameObject)Instantiate(object_to_spawn, spawn_location, object_rotation);

        spawned_object.transform.parent = parent_object.transform;

        NetworkServer.Spawn(spawned_object);
    }

    // Spawn Projectile and set all according parameters from the projectile attack script that launched the projectile

    [Command]

    public void CmdSpawnProjectile(GameObject projectile, Vector2 launch_location, GameObject projectile_launcher)
    {
        var clone = (GameObject)Instantiate(projectile, launch_location, projectile_launcher.GetComponent<ProjectileAttack>().transform.rotation);

        // sets whether the projectile moves independently or is a parent of the launcher
        if (projectile_launcher.GetComponent<ProjectileAttack>().projectile_parent)
        {
            clone.transform.parent = projectile_launcher.transform;
        }

        // sets whether the damage of the projectile is determined by the prefab or the attack script (since multiple projectile
        // attacks may use the same projectile)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_damage)
        {
            clone.GetComponent<Projectile>().damage_val = projectile_launcher.GetComponent<ProjectileAttack>().variable_damage_value;
        }

        // sets whether the speed of the projectile is determined by the prefab or the attack script (similar to variable_damage)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_speed)
        {
            clone.GetComponent<Projectile>().projectile_speed = projectile_launcher.GetComponent<ProjectileAttack>().variable_speed_value;
        }

        // sets which player the projectile belongs to (ENEMY on the right side of screen, Non-ENEMY on the left)
        clone.GetComponent<Projectile>().enemy_check = projectile_launcher.GetComponent<ProjectileAttack>().enemy_check;

        // sets the projectile to be on the same layer as the launcher (for lane sort purposes)
        clone.GetComponent<SpriteRenderer>().sortingLayerName = projectile_launcher.GetComponent<SpriteRenderer>().sortingLayerName;

        // sets whether the projectile appears over top of bots
        if (projectile_launcher.GetComponent<ProjectileAttack>().attack_over)
        {
            clone.GetComponent<SpriteRenderer>().sortingOrder = (projectile_launcher.GetComponent<SpriteRenderer>().sortingOrder + 10);
        }

        // sets whether the projectile has a variable range (lifespan)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_range == true)
        {
            if (projectile_launcher.GetComponent<ProjectileAttack>().range_stat_dependent)
            {
                if (projectile_launcher.transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE < 1)
                {
                    clone.GetComponent<DestroyAfterTime>().LifeTime = 0.3f * Time.deltaTime * 50;
                }
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectile_launcher.transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE * 0.3f * Time.deltaTime * 50;
            }
            else
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectile_launcher.GetComponent<ProjectileAttack>().variable_range_value * 0.3f * Time.deltaTime * 50;
            }
        }

        // sets the projectile's height (y position)
        projectile.GetComponent<Projectile>().ground_y_position = projectile_launcher.GetComponent<ProjectileAttack>().leg_component.transform.position.y;

        // synchronizes/spawns the projectile on all clients (both server and non-server)
        NetworkServer.Spawn(clone);
    }

    // Projectile handles most attacks but tesla attacks were a fair bit more complicated and thus have their own spawn function.
    // Lightning consists of a unique start segment (with unique animation) a looping middle section (of variable length) and an end section (unique)

    [Command]

    public void CmdSpawnLightning(GameObject lightning_object, GameObject launch_point, int lightning_damage, bool enemy_check, int power)
    {
        // spawns the lightning start object and sets damage, enemy and collider size appropriately.
        var lightning_start_segment = (GameObject)Instantiate(lightning_object, launch_point.transform.position, launch_point.transform.rotation);
        lightning_start_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
        lightning_start_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;
        lightning_start_segment.GetComponent<LightningDamage>().lightning_start = true;
        lightning_start_segment.GetComponent<BoxCollider2D>().size = new Vector2((power + 2) * 26, 60);
        lightning_start_segment.GetComponent<BoxCollider2D>().offset = new Vector2(13 + 13 * (power), 10);

        // spawn the lightning start on the server
        NetworkServer.Spawn(lightning_start_segment);

        // using the enemy check, move the launch point forwards (to the right for non_enemy and to the left for enemy)
        if (!enemy_check)
        {
            lightning_dist = launch_point.transform.position.x + 26;
        }
        else
        {
            lightning_dist = launch_point.transform.position.x - 26;
        }

        // depending on the lightning power, spawn a number of middle lightning segments moving down further each time.
        for (int lightning_power = 0; lightning_power < power; lightning_power++)
        {
            lightning_point = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);
            var lightning_middle_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
            lightning_middle_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
            lightning_middle_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;

            lightning_middle_segment.GetComponent<Collider2D>().enabled = false;

            if (!enemy_check)
            {
                lightning_dist += 26;
            }
            else
            {
                lightning_dist -= 26;
            }

            // use an alternator so odd and even lightning middle segments are different visually

            if (alternator == false)
            {
                lightning_middle_segment.GetComponent<LightningDamage>().lightning_middle1 = true;
                alternator = true;
            }
            else if (alternator == true)
            {
                lightning_middle_segment.GetComponent<LightningDamage>().lightning_middle2 = true;
                lightning_middle_segment.GetComponent<Collider2D>().enabled = false;
                alternator = false;
            }

            // Spawn middle sections on the server 
            NetworkServer.Spawn(lightning_middle_segment);
        }

        // spawn lightning end section
        lightning_point = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);

        var lightning_end_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
        lightning_end_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
        lightning_end_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;
        lightning_end_segment.GetComponent<LightningDamage>().lightning_end = true;

        lightning_end_segment.GetComponent<Collider2D>().enabled = false;

        NetworkServer.Spawn(lightning_end_segment);
    }

    // Whenever damage is dealt, a small graphic appears with the amount of damage dealt. This function handles
    // the spawn of that graphic

    [Command]

    public void CmdSpawnDamageValues(
        GameObject damage_object,
        Vector2 damage_target,
        int damage_value
        )
    {
        //Debug.Log("effects created");

        // instantiate the damage indicator gameobject
        var damage = (GameObject)Instantiate(damage_object, damage_target, Quaternion.Euler(Vector3.zero));

        // set the damage indicator to the correct value
        damage.GetComponent<DamageValues>().damage_value = damage_value;

        if (damage != null)
        {
            NetworkServer.Spawn(damage);
        }
    }

    // spawn impact objects. If the projectile does not pierce then destroy the projectile that caused the impact
    [Command]
    public void CmdSpawnImpact(
        bool use_impact,
        GameObject impact_object,
        Vector2 impact_position,
        Quaternion impact_rotation,
        bool piercing,
        GameObject projectile)
    {

        if (use_impact)
        {
            var impact = (GameObject)Instantiate(impact_object, impact_position, impact_rotation);

            NetworkServer.Spawn(impact);
        }

        if (!piercing)
        {
            NetworkServer.Destroy(projectile);
        }
    }

    // Given a part_name and an index, search the appropriate library of the 
    // associated type until a matching name is found and return the index of the part
    // in that part library.

    public int search_library(string part_name, int part_type)
    {

        for (int search_index = 0; search_index < part_library[part_type].part_library.Length; search_index++)
        {
            //Debug.Log(part_type);
            //Debug.Log(search_index);
            //Debug.Log(part_library[part_type].part_library[search_index]);

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
}
