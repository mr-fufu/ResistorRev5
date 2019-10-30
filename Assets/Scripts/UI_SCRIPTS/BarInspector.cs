using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarInspector : MonoBehaviour
{
    public GameObject spawn_controller;
    public GameObject inspector;
    public GameObject display_point;
    public GameObject credit_value;

    public GameObject bar_box_1;
    public GameObject bar_box_2;
    public GameObject bar_box_3;
    public GameObject bar_box_4;

    private bool[] clone_check;

    private GameObject[] bot_loadout;
    private GameObject[] bar_box;
    private int part_count;
    private GameObject[] contained_parts;

    // Start is called before the first frame update
    void Start()
    {
        bot_loadout = new GameObject[4];
        bar_box = new GameObject[4];

        bar_box[0] = bar_box_1;
        bar_box[1] = bar_box_2;
        bar_box[2] = bar_box_3;
        bar_box[3] = bar_box_4;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("1"))
        {
            bar_box_1.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_1.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("2"))
        {
            bar_box_2.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_2.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("3"))
        {
            bar_box_3.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_3.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("4"))
        {
            bar_box_4.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_4.GetComponent<BarSelect>().box_no;
        }

        if (spawn_controller.GetComponent<SpawnController>().spawn_bot_1 != null)
        {
            if (spawn_controller.GetComponent<SpawnController>().spawn_bot_1.transform.GetChild(0).childCount != 0)
            {
                bot_loadout[0] = spawn_controller.GetComponent<SpawnController>().spawn_bot_1.transform.GetChild(0).transform.GetChild(0).gameObject;
            }
            else
            {
                bot_loadout[0] = null;
            }
        }

        if (spawn_controller.GetComponent<SpawnController>().spawn_bot_2 != null)
        {
            if (spawn_controller.GetComponent<SpawnController>().spawn_bot_2.transform.GetChild(0).childCount != 0)
            {
                bot_loadout[1] = spawn_controller.GetComponent<SpawnController>().spawn_bot_2.transform.GetChild(0).transform.GetChild(0).gameObject;
            }
            else
            {
                bot_loadout[1] = null;
            }
        }

        if (spawn_controller.GetComponent<SpawnController>().spawn_bot_3 != null)
        {

            if (spawn_controller.GetComponent<SpawnController>().spawn_bot_3.transform.GetChild(0).childCount != 0)
            {
                bot_loadout[2] = spawn_controller.GetComponent<SpawnController>().spawn_bot_3.transform.GetChild(0).transform.GetChild(0).gameObject;
            }
            else
            {
                bot_loadout[2] = null;
            }
        }

        if (spawn_controller.GetComponent<SpawnController>().spawn_bot_4 != null)
        {
            if (spawn_controller.GetComponent<SpawnController>().spawn_bot_4.transform.GetChild(0).childCount != 0)
            {
                bot_loadout[3] = spawn_controller.GetComponent<SpawnController>().spawn_bot_4.transform.GetChild(0).transform.GetChild(0).gameObject;
            }
            else
            {
                bot_loadout[3] = null;
            }
        }

        var mouse_position = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray_hit = (Physics2D.Raycast(mouse_position, Vector2.zero, Mathf.Infinity));
        if (ray_hit.collider != null)
        {
            for (int check_no = 0; check_no < 4; check_no++)
            {
                if (ray_hit.collider.gameObject == bar_box[check_no])
                {
                    bar_box[check_no].GetComponent<BarSelect>().moused_over = true;

                    if (bot_loadout[check_no] != null)
                    {
                        inspector.SetActive(true);
                        inspector.transform.position = new Vector2(bar_box[check_no].transform.position.x, inspector.transform.position.y);

                        bot_loadout[check_no].transform.position = display_point.transform.position;
                        bot_loadout[check_no].transform.localScale = display_point.transform.localScale;
                        //bot_loadout[check_no].

                        credit_value.GetComponent<UnityEngine.UI.Text>().text = "" + bot_loadout[check_no].GetComponent<StandardStatBlock>().COST;
                    }
                }
                else
                {
                    bar_box[check_no].GetComponent<BarSelect>().moused_over = false;

                    if (bot_loadout[check_no] != null)
                    {
                        bot_loadout[check_no].transform.localPosition = new Vector2(0,0);
                    }
                }

                if (ray_hit.collider.gameObject != bar_box[0] && ray_hit.collider.gameObject != bar_box[1] && ray_hit.collider.gameObject != bar_box[2] && ray_hit.collider.gameObject != bar_box[3])
                {

                }
            }
        }
        else
        {
            inspector.SetActive(false);
            for (int box_no = 0; box_no < 4; box_no++)
            {
                bar_box[box_no].GetComponent<BarSelect>().moused_over = false;

                if (bot_loadout[box_no] != null)
                {
                    bot_loadout[box_no].transform.localPosition = new Vector2(0,0);
                }
            }
        }
    }
}
