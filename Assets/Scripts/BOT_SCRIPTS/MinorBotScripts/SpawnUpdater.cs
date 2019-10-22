using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUpdater : MonoBehaviour
{
    public bool spawned;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        spawned = transform.parent.transform.parent.GetComponent<MeleeAttack>().spawned;
        if (spawned)
        {
            anim.speed = transform.parent.transform.parent.GetComponent<Animator>().speed;
            anim.SetBool("spawned", true);
        }
    }
}
