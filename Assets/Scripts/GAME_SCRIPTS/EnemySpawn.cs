using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject credits;

    public GameObject wave_1_enemy_1;
    public GameObject wave_1_enemy_2;
    public GameObject wave_1_enemy_3;
    public GameObject wave_1_enemy_4;

    public GameObject wave_2_enemy_1;
    public GameObject wave_2_enemy_2;
    public GameObject wave_2_enemy_3;
    public GameObject wave_2_enemy_4;

    public Transform enemy_spawn_lane_1;
    public Transform enemy_spawn_lane_2;
    public Transform enemy_spawn_lane_3;

    public GameObject credit_counter;
    public Color enemy_color;

    private GameObject bot_to_spawn;

    private GameObject[] wave_1;
    private GameObject[] wave_2;
    private Transform[] spawn_lane;
    private string[] lane_name;

    private int lane_select;
    private int spawn_select;

    public int spawn_time;
    private int spawn_clock;

    private int enemy_clone_part_count;
    private GameObject reference_root;
    private GameObject[] contained_parts;
    private bool complete;

    // Start is called before the first frame update
    void Start()
    {
        spawn_clock = spawn_time - 50;

        wave_1 = new GameObject[4];
        wave_2 = new GameObject[4];
        spawn_lane = new Transform[3];
        lane_name = new string[3];

        wave_1[0] = wave_1_enemy_1;
        wave_1[1] = wave_1_enemy_2;
        wave_1[2] = wave_1_enemy_3;
        wave_1[3] = wave_1_enemy_4;

        wave_2[0] = wave_2_enemy_1;
        wave_2[1] = wave_2_enemy_2;
        wave_2[2] = wave_2_enemy_3;
        wave_2[3] = wave_2_enemy_4;

        spawn_lane[0] = enemy_spawn_lane_1;
        spawn_lane[1] = enemy_spawn_lane_2;
        spawn_lane[2] = enemy_spawn_lane_3;

        lane_name[0] = "LANE1";
        lane_name[1] = "LANE2";
        lane_name[2] = "LANE3";
    }

    // Update is called once per frame
    void Update()
    {
        spawn_clock++;

        if (spawn_clock > spawn_time)
        {
            spawn_select = Random.Range(0, 4);
            bot_to_spawn = wave_1[spawn_select];

            if (bot_to_spawn.GetComponent<StandardStatBlock>().COST <= credits.GetComponent<CreditCounter>().credit_value)
            {
                credits.GetComponent<CreditCounter>().credit_value -= bot_to_spawn.GetComponent<StandardStatBlock>().COST;

                lane_select = Random.Range(0, 3);

                var enemy_clone = Instantiate(bot_to_spawn, spawn_lane[lane_select].position, spawn_lane[lane_select].rotation);
                enemy_clone.GetComponent<StandardStatBlock>().ENEMY = true;
                enemy_clone.GetComponent<AutoMove>().spawned = true;
                enemy_clone.gameObject.tag = "BOT_Enemy";
                enemy_clone.gameObject.layer = 13;
                enemy_clone.GetComponent<Collider2D>().enabled = true;

                if (enemy_clone.GetComponent<StandardStatBlock>().melee_components > 0)
                {
                    enemy_clone.GetComponent<Collider2D>().offset = new Vector2(-5, 25);
                    enemy_clone.GetComponent<BoxCollider2D>().size = new Vector2(25, 50);
                }
                else
                {
                    enemy_clone.GetComponent<Collider2D>().offset = new Vector2(6, 25);
                    enemy_clone.GetComponent<BoxCollider2D>().size = new Vector2(40, 50);
                }

                enemy_clone.GetComponent<SpriteRenderer>().sortingLayerName = lane_name[lane_select];
                enemy_clone.GetComponent<SpriteRenderer>().color = enemy_color;

                change_children(enemy_clone, enemy_color, lane_name[lane_select]);

                spawn_clock = 0;
            }

            
        }
    }

    public void change_children(GameObject selected_object, Color enemy_color, string lane_name)
    {
        enemy_clone_part_count = selected_object.gameObject.transform.childCount;

        if (enemy_clone_part_count != 0)
        {
            for (int var = 0; var < enemy_clone_part_count; var++)
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
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = enemy_color;
                                change_children(selected_object.transform.GetChild(var).GetChild(0).gameObject, enemy_color, lane_name);
                                enemy_clone_part_count = selected_object.transform.childCount;
                            }
                            else
                            {
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = lane_name;
                                selected_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = enemy_color;
                            }
                        }
                    }
                }

                

            }
        }
    }
}