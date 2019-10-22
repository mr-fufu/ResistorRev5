using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeDetection : MonoBehaviour
{
    public bool scanned;
    public bool fixed_range;
    public bool range_overide;
    public int range_overide_value;
    public bool scan_overide;
    public int scan_overide_value;

    private Collider2D[] check_colliders;
    private int scan_size = 20;
    private ContactFilter2D collider_filter = new ContactFilter2D();

    private float total_range;
    private int range_value;
    private int speed_value;

    private int empty_count;

    private GameObject double_parent;
    private GameObject quadruple_parent;
    private bool spawned;
    private ProjectileAttack weapon_attack;
    private bool enemy_check;
    private string check_tag;

    // Start is called before the first frame update
    void Start()
    {
        collider_filter = collider_filter.NoFilter();
        double_parent = transform.parent.transform.parent.gameObject;
        weapon_attack = double_parent.GetComponent<ProjectileAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if (double_parent.GetComponent<PartStats>().attached == true)
        {
            quadruple_parent = double_parent.transform.parent.transform.parent.gameObject;
            spawned = quadruple_parent.GetComponent<StandardStatBlock>().spawned;
            enemy_check = quadruple_parent.GetComponent<StandardStatBlock>().ENEMY;

            if (!enemy_check)
            {
                check_tag = "BOT_Enemy";
            }
            else if (enemy_check)
            {
                check_tag = "BOT_Player";
            }
        }

        if (spawned)
        {

            if (!fixed_range)
            {
                if (!weapon_attack.variable_speed)
                {
                    speed_value = double_parent.GetComponent<ProjectileAttack>().projectile.GetComponent<Projectile>().projectile_speed;
                }
                else
                {
                    speed_value = weapon_attack.variable_speed_value;
                }

                if (weapon_attack.variable_range && weapon_attack.range_stat_dependent)
                {
                    range_value = quadruple_parent.GetComponent<StandardStatBlock>().RANGE;
                }
                else if (weapon_attack.variable_range && !weapon_attack.range_stat_dependent)
                {
                    range_value = weapon_attack.variable_range_value;
                }
                else if (range_overide)
                {
                    range_value = range_overide_value;
                }
                else
                {
                    range_value = 1;
                }

                total_range = (speed_value * 1.8f) * (range_value * 1.8f);

                if (total_range > 500)
                {
                    total_range = 500;
                }

                if (scan_overide)
                {
                    total_range = scan_overide_value;
                }

                GetComponent<BoxCollider2D>().size = new Vector2(total_range, 30);
                GetComponent<BoxCollider2D>().offset = new Vector2(total_range / 2, 0);
            }

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
                    if (check_colliders[checkVar].gameObject.tag == check_tag)
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
}
