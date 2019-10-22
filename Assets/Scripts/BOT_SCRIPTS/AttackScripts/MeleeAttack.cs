using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public bool spawned;
    public int damage_val;
    public float attack_speed;
    public bool EngagedCheck;
    public bool attached;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.speed = attack_speed;

        attached = GetComponent<PartStats>().attached;

        if (attached)
        {
            spawned = transform.parent.transform.parent.GetComponent<StandardStatBlock>().spawned;
        }

        if (spawned)
        {
            EngagedCheck = transform.parent.transform.parent.GetComponent<StandardStatBlock>().engaged_check;

            if (EngagedCheck)
            {
                anim.SetBool("Engagement", true);
            }
            else
            {
                anim.SetBool("Engagement", false);
            }
        }
    }
}
