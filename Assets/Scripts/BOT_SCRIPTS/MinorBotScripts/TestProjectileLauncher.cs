using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectileLauncher : MonoBehaviour {

    public int attack_speed;
    public GameObject projectile;
    private int reloadtime = 100;
    private bool enemy_check;
    public Transform launch_point;

    public bool variable_range;
    public int variable_range_value;
    public bool range_stat_dependent;

    public bool variable_speed;
    public int variable_speed_value;

    public bool variable_y;
    public float variable_y_value;

    public bool variable_damage;
    public int variable_damage_value;

    private Vector2 launch_location;

    // Use this for initialization
    void Start ()
    {
        enemy_check = false;
    }
	
	// Update is called once per frame
	void Update () {

            reloadtime -= attack_speed;
        if (reloadtime <= 0)
        {
            launch_location = launch_point.position;

            if (variable_y)
            {
                launch_location[1] += Random.Range(-variable_y_value, variable_y_value);
            }

            var clone = (GameObject)Instantiate(projectile, launch_location, launch_point.rotation);

            if (variable_damage)
            {
                clone.GetComponent<Projectile>().damage_val = variable_damage_value;
            }

            if (variable_speed)
            {
                clone.GetComponent<Projectile>().projectile_speed = variable_speed_value;
            }

            clone.GetComponent<Projectile>().enemy_check = enemy_check;

            if (variable_range == true)
            {
                if (range_stat_dependent)
                {
                    clone.GetComponent<DestroyAfterTime>().LifeTime = transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE * 0.1f;
                }
                else
                {
                    clone.GetComponent<DestroyAfterTime>().LifeTime = variable_range_value * 0.1f;
                }
            }
            reloadtime = 100;
        }
	}
}
