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

    private int emptyCount;

    private GameObject doubleParent;
    private GameObject quadrupleParent;
    private ProjectileAttack weaponAttack;
    private Projectile projectile;
    private bool enemy;
    private string checkTag;

    // Start is called before the first frame update
    void Start()
    {
        collider_filter = collider_filter.NoFilter();
        doubleParent = transform.parent.transform.parent.gameObject;
        weaponAttack = doubleParent.GetComponent<ProjectileAttack>();
        projectile = weaponAttack.projectile.GetComponent<Projectile>();
        quadrupleParent = doubleParent.transform.parent.transform.parent.gameObject;
        enemy = quadrupleParent.GetComponent<StandardStatBlock>().ENEMY;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemy)
        {
            checkTag = "BOT_Enemy";
        }
        else if (enemy)
        {
            checkTag = "BOT_Player";
        }

        if (!fixed_range)
        {
            if (!weaponAttack.variable_speed)
            {
                if (projectile.projectile_speed > 0) {
                    speed_value = projectile.projectile_speed;
                }
                else
                {
                    speed_value = 1;
                }
            }
            else
            {
                speed_value = weaponAttack.variable_speed_value;
            }

            if (weaponAttack.variable_range && weaponAttack.range_stat_dependent)
            {
                range_value = quadrupleParent.GetComponent<StandardStatBlock>().RANGE;
            }
            else if (weaponAttack.variable_range && !weaponAttack.range_stat_dependent)
            {
                range_value = weaponAttack.variable_range_value;
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

        emptyCount = 0;

        gameObject.GetComponent<Collider2D>().OverlapCollider(collider_filter, check_colliders);

        for (int checkVar = 0; checkVar < scan_size; checkVar++)
        {
            if (check_colliders[checkVar] != null)
            {
                if (check_colliders[checkVar].gameObject.tag == checkTag)
                {
                    scanned = true;
                }
                else
                {
                    emptyCount++;
                }
            }
            else
            {
                emptyCount++;
            }
        }

        if (emptyCount == scan_size)
        {
            scanned = false;
        }
    }
}
