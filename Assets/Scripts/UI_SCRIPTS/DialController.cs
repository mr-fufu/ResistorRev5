using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialController : MonoBehaviour {

    public GameObject camera_object;

    public int dial_state = 0;
    public int max_dial_states;
    public GameObject leg_viewer;
    public GameObject torso_viewer;
    public GameObject head_viewer;
    public GameObject arm_viewer;
    public GameObject weapon_viewer;
    public GameObject armor_viewer;

    private GameObject[] part_viewer;
    private bool viewer_change;

    public Scrollbar scroll;

    void Start () {

        part_viewer = new GameObject[6];

        part_viewer[0] = leg_viewer;
        part_viewer[1] = torso_viewer;
        part_viewer[2] = head_viewer;
        part_viewer[3] = arm_viewer;
        part_viewer[4] = weapon_viewer;
        part_viewer[5] = armor_viewer;

        ChangeViewer();
    }

    private void Update()
    {

    }

    void OnMouseDown () {
        if (camera_object.GetComponent<WorkshopDrag>().tutorial_complete)
        {
            dial_state++;

            if (dial_state >= max_dial_states)
            {
                dial_state = 0;
            }

            ChangeViewer();
        }
    }

    public void ChangeViewer()
    {
        for (int dial_index = 0; dial_index < max_dial_states; dial_index++)
        {
            if (dial_index != dial_state)
            {
                part_viewer[dial_index].SetActive(false);
            }
        }
        part_viewer[dial_state].SetActive(true);

        scroll.value = part_viewer[dial_state].GetComponentInChildren<CustomScroll>().scroll_holdover;
    }
}
