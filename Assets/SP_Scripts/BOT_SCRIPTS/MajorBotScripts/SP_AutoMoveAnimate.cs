using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_AutoMoveAnimate : MonoBehaviour
{
    private Animator Anim;
    public bool animate;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = gameObject.GetComponent<AutoMove>().animated_move_speed;

        Anim = GetComponent<Animator>();
        Anim.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        animate = gameObject.GetComponent<AutoMove>().animate_check;

        if (animate)
        {
            Anim.SetBool("WalkState", true);
        }
        else if (!animate)
        {
            Anim.SetBool("WalkState", false);
        }

    }
}
