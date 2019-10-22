using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocker : MonoBehaviour
{
    public bool spawn_blocked;
    private Collider2D[] check_colliders;
    private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();
    private int empty_count;

    // Start is called before the first frame update
    void Start()
    {
        spawn_blocked = false;
        collider_filter = collider_filter.NoFilter();
    }

    // Update is called once per frame
    void Update()
    {
        check_colliders = new Collider2D[scan_size];

        for (int setVar = 0; setVar < scan_size; setVar++)
        {
            check_colliders[setVar] = null;
        }

        empty_count = 0;

        gameObject.GetComponent<Collider2D>().OverlapCollider(collider_filter, check_colliders);

        // check through all elements of check_colliders
        // to see if any of them is the engaged_target


        for (int checkVar = 0; checkVar < scan_size; checkVar++)
        {
            if (check_colliders[checkVar] != null)
            {
                if (check_colliders[checkVar].gameObject.tag == "BOT_Player")
                {
                    spawn_blocked = true;
                }
                else
                {
                    empty_count++;
                }
            }
            else
            {
                empty_count++;
            }
        }

        if (empty_count == scan_size)
        {
            spawn_blocked = false;
        }
    }
}
