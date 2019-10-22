using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoMoveAnimate : NetworkBehaviour
{

    private Animator Anim;
    [SyncVar] private bool animate;

    private float speed;

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
