using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissile : MonoBehaviour
{
    public float y_range;
    public float lower_range;

    public bool flip;
    public float delta = 0;
    public float rate = 1f;

    public float datum;

    public Animator anim;

    public float decay;
    private Vector2 hold_position;

    // Start is called before the first frame update
    void Start()
    {
        datum = gameObject.transform.position.y;

        anim = gameObject.GetComponent<Animator>();

        flip = true;

        lower_range = -y_range;
    }

    // Update is called once per frame
    void Update()
    {

        if (flip)
        {
            delta += rate;
            anim.SetBool("MoveUp", true);
            anim.SetBool("MoveDown", false);
        }
        else
        {
            delta -= rate;
            anim.SetBool("MoveUp", false);
            anim.SetBool("MoveDown", true);
        }

        if (delta > y_range)
        {
            anim.SetBool("MoveUp", false);
            anim.SetBool("MoveDown", false);
            flip = false;
        }
        else if (delta < lower_range)
        {
            anim.SetBool("MoveUp", false);
            anim.SetBool("MoveDown", false);
            flip = true;
        }

        //decay -= 0.001f;

        hold_position = transform.position;
        hold_position.y = datum + delta;
        transform.position = hold_position;
    }

}
