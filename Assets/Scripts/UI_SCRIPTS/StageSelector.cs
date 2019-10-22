using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelector : MonoBehaviour
{
    // fairly simple script, sets one of four stages active based on which is currently selected. Stages are
    // root objects upon which workshop bots are built, selected box is pulled from BarSelectParentLink

    public GameObject stage_area_1;
    public GameObject stage_area_2;
    public GameObject stage_area_3;
    public GameObject stage_area_4;

    public bool change_scene;

    private int selected_box;

    void Start()
    {
        stage_area_1.SetActive(true);
        stage_area_2.SetActive(true);
        stage_area_3.SetActive(true);
        stage_area_4.SetActive(true);
    }

    void Update()
    {
        // pull selected box from attached bar select parent link script (contains only a single int selected_box),
        // selection and changing the value is done by the boxes themselves which link to the script
        selected_box = gameObject.GetComponent<BarSelectParentLink>().selected_box;

        // if not currently in the process of changing to another scene
        if (!change_scene)
        {
            // 4 big ol if statements to set the appropriate stage gameobject collider and its contents active.
            // could be done more efficiently by arrays, change if time leftover or something
            if (selected_box == 1)
            {
                stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = true;
                stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

                if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                }

                if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

            }
            else if (selected_box == 2)
            {
                stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = true;
                stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

                if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                }

                if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else if (selected_box == 3)
            {
                stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = true;
                stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

                if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                }

                if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else if (selected_box == 4)
            {
                stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
                stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = true;

                if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                }

                if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
                {
                    stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        //-------------------------------------------------------------------------
        // if the scene is currently changing then disable all colliders and set all built bots to true
        else if (change_scene)
        {
            stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
            stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
            stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
            stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

            if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
            {
                stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }

            if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
            {
                stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }

            if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
            {
                stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }

            if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
            {
                stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }
        }

    }
    // basically the same as the change_scene if condition only in the form of a function to be called by
    // the scene management script if needed
    public void set_all_active()
    {
        stage_area_1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
        stage_area_2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
        stage_area_3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;
        stage_area_4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

        if (stage_area_1.transform.GetChild(0).transform.childCount != 0)
        {
            stage_area_1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }

        if (stage_area_2.transform.GetChild(0).transform.childCount != 0)
        {
            stage_area_2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }

        if (stage_area_3.transform.GetChild(0).transform.childCount != 0)
        {
            stage_area_3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }

        if (stage_area_4.transform.GetChild(0).transform.childCount != 0)
        {
            stage_area_4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
