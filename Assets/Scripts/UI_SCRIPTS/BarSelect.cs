using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarSelect : MonoBehaviour {

    public bool moused_over;
    public int box_no;
    public int selected_box;
    private SpriteRenderer box_sprite;
    public Color selected_color;
    public Color mouseover_color;
    private Color unselected_color = new Color(255, 255, 255);

    void Start ()
    {
        box_sprite = GetComponent<SpriteRenderer>();
	}

    void Update()
    {
        selected_box = transform.parent.gameObject.GetComponent<BarSelectParentLink>().selected_box;

        if (selected_box == box_no)
        {
            box_sprite.color = selected_color;
        }
        else
        {
            if (moused_over)
            {
                box_sprite.color = mouseover_color;
            }
            else
            {
                box_sprite.color = unselected_color;
            }
        }
    }

    private void OnMouseDown()
    {
        if (selected_box != box_no)
        {
            transform.parent.gameObject.GetComponent<BarSelectParentLink>().selected_box = box_no;
        }
    }

    void OnMouseOver()
    {

    }
}

