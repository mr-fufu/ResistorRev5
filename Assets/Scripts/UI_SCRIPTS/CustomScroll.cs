using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScroll : MonoBehaviour
{
    public Scrollbar scroll;
    public int scroll_length;
    private int scroll_dist;
    private float scroll_position;
    private Vector2 self_transform;
    private Vector2 original_position;
    public float scroll_holdover;
    public int window_length;

    public WorkshopDrag workshop;

    // Start is called before the first frame update
    void Start()
    {
        original_position = new Vector2(transform.position.x, transform.position.y);
        self_transform = new Vector2(transform.position.x, transform.position.y);
        scroll_dist = (scroll_length - 3)*window_length;

        scroll_holdover = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (workshop.tutorial_complete)
        {
            scroll.value -= Input.mouseScrollDelta.y * (0.2f / scroll_length);
            scroll_position = (scroll.value * scroll_dist);
        }

        if (scroll_position <= 0)
        {
            scroll.value = 0;
            scroll_position = 0;
            self_transform.y = original_position.y;
            transform.position = self_transform;
        }
        else if (scroll_position >= scroll_dist)
        {
            scroll.value = 1;
            scroll_position = scroll_dist;
            self_transform.y = original_position.y + scroll_position;
            transform.position = self_transform;
        }
        else
        {
            self_transform.y = original_position.y + scroll_position;
            transform.position = self_transform;
        }

        scroll_holdover = scroll.value;
    }
}
