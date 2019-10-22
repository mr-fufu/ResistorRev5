using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousAttack : MonoBehaviour
{
    public bool EngagedCheck;
    public bool continuous_attack;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EngagedCheck = GetComponent<MeleeAttack>().EngagedCheck;

        if (EngagedCheck)
        {
            anim.SetBool("ContinuousAttack", true);
        }
        if (!EngagedCheck)
        {
            anim.SetBool("ContinuousAttack", false);
        }
    }
}
