using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRange : MonoBehaviour
{
    public bool scanned;

    private Collider2D[] check_colliders;
    private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();

    private float total_range;

    private bool enemy;

    private int empty_count;

    private GameObject double_parent;

    private bool spawned;

    // Start is called before the first frame update
    void Start()
    {
        collider_filter = collider_filter.NoFilter();

        double_parent = transform.parent.transform.parent.gameObject;

        enemy = double_parent.GetComponent<LightningAttack>().enemy_check;

        total_range = ((double_parent.GetComponent<LightningAttack>().power + 1) * 26f);

        GetComponent<BoxCollider2D>().size = new Vector2(total_range, 30);
        GetComponent<BoxCollider2D>().offset = new Vector2(total_range / 2, 0);
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

        for (int checkVar = 0; checkVar < scan_size; checkVar++)
        {
            if (check_colliders[checkVar] != null)
            {
                if ((!enemy && check_colliders[checkVar].gameObject.tag == "BOT_Enemy") || enemy && check_colliders[checkVar].gameObject.tag == "BOT_Player")
                {
                    scanned = true;
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
            scanned = false;
        }
    }      
}
