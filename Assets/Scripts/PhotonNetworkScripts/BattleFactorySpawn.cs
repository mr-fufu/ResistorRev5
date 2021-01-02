using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFactorySpawn : MonoBehaviour
{
    private float lightning_dist;
    private Vector3 lightning_point;
    private bool alternator;
    
    public static BattleFactorySpawn instance;

    private void Start()
    {
        if(instance != null)
        {
            instance = this;
        }
    }

    public void SpawnGeneric(GameObject object_to_spawn, Vector2 spawn_location, GameObject parent_object, Quaternion object_rotation)
    {
        GameObject spawned_object = Instantiate(object_to_spawn, spawn_location, object_rotation);
        spawned_object.transform.parent = parent_object.transform;
    }

    // Spawn Projectile and set all according parameters from the projectile attack script that launched the projectile

    public void SpawnProjectile(GameObject projectile, Vector2 launch_location, GameObject projectile_launcher)
    {
        GameObject clone = Instantiate(projectile, launch_location, projectile_launcher.GetComponent<ProjectileAttack>().transform.rotation);

        // sets whether the projectile moves independently or is a parent of the launcher
        if (projectile_launcher.GetComponent<ProjectileAttack>().projectile_parent)
        {
            clone.transform.parent = projectile_launcher.transform;
        }

        // sets whether the damage of the projectile is determined by the prefab or the attack script (since multiple projectile
        // attacks may use the same projectile)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_damage)
        {
            clone.GetComponent<Projectile>().damage_val = projectile_launcher.GetComponent<ProjectileAttack>().variable_damage_value;
        }

        // sets whether the speed of the projectile is determined by the prefab or the attack script (similar to variable_damage)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_speed)
        {
            clone.GetComponent<Projectile>().projectile_speed = projectile_launcher.GetComponent<ProjectileAttack>().variable_speed_value;
        }

        // sets which player the projectile belongs to (ENEMY on the right side of screen, Non-ENEMY on the left)
        clone.GetComponent<Projectile>().enemy_check = projectile_launcher.GetComponent<ProjectileAttack>().enemy_check;

        // sets the projectile to be on the same layer as the launcher (for lane sort purposes)
        clone.GetComponent<SpriteRenderer>().sortingLayerName = projectile_launcher.GetComponent<SpriteRenderer>().sortingLayerName;

        // sets whether the projectile appears over top of bots
        if (projectile_launcher.GetComponent<ProjectileAttack>().attack_over)
        {
            clone.GetComponent<SpriteRenderer>().sortingOrder = (projectile_launcher.GetComponent<SpriteRenderer>().sortingOrder + 10);
        }

        // sets whether the projectile has a variable range (lifespan)
        if (projectile_launcher.GetComponent<ProjectileAttack>().variable_range == true)
        {
            if (projectile_launcher.GetComponent<ProjectileAttack>().range_stat_dependent)
            {
                if (projectile_launcher.transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE < 1)
                {
                    clone.GetComponent<DestroyAfterTime>().LifeTime = 0.3f * Time.deltaTime * 50;
                }
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectile_launcher.transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE * 0.3f * Time.deltaTime * 50;
            }
            else
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectile_launcher.GetComponent<ProjectileAttack>().variable_range_value * 0.3f * Time.deltaTime * 50;
            }
        }

        // sets the projectile's height (y position)
        projectile.GetComponent<Projectile>().ground_y_position = projectile_launcher.GetComponent<ProjectileAttack>().leg_component.transform.position.y;
    }

    // Projectile handles most attacks but tesla attacks were a fair bit more complicated and thus have their own spawn function.
    // Lightning consists of a unique start segment (with unique animation) a looping middle section (of variable length) and an end section (unique)

    public void SpawnLightning(GameObject lightning_object, GameObject launch_point, int lightning_damage, bool enemy_check, int power)
    {
        // spawns the lightning start object and sets damage, enemy and collider size appropriately.
        var lightning_start_segment = (GameObject)Instantiate(lightning_object, launch_point.transform.position, launch_point.transform.rotation);
        lightning_start_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
        lightning_start_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;
        lightning_start_segment.GetComponent<LightningDamage>().lightning_start = true;
        lightning_start_segment.GetComponent<BoxCollider2D>().size = new Vector2((power + 2) * 26, 60);
        lightning_start_segment.GetComponent<BoxCollider2D>().offset = new Vector2(13 + 13 * (power), 10);

        // using the enemy check, move the launch point forwards (to the right for non_enemy and to the left for enemy)
        if (!enemy_check)
        {
            lightning_dist = launch_point.transform.position.x + 26;
        }
        else
        {
            lightning_dist = launch_point.transform.position.x - 26;
        }

        // depending on the lightning power, spawn a number of middle lightning segments moving down further each time.
        for (int lightning_power = 0; lightning_power < power; lightning_power++)
        {
            lightning_point = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);
            var lightning_middle_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
            lightning_middle_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
            lightning_middle_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;

            lightning_middle_segment.GetComponent<Collider2D>().enabled = false;

            if (!enemy_check)
            {
                lightning_dist += 26;
            }
            else
            {
                lightning_dist -= 26;
            }

            // use an alternator so odd and even lightning middle segments are different visually

            if (alternator == false)
            {
                lightning_middle_segment.GetComponent<LightningDamage>().lightning_middle1 = true;
                alternator = true;
            }
            else if (alternator == true)
            {
                lightning_middle_segment.GetComponent<LightningDamage>().lightning_middle2 = true;
                lightning_middle_segment.GetComponent<Collider2D>().enabled = false;
                alternator = false;
            }
        }

        // spawn lightning end section
        lightning_point = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);

        var lightning_end_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
        lightning_end_segment.GetComponent<LightningDamage>().damage_val = lightning_damage;
        lightning_end_segment.GetComponent<LightningDamage>().enemy_check = enemy_check;
        lightning_end_segment.GetComponent<LightningDamage>().lightning_end = true;

        lightning_end_segment.GetComponent<Collider2D>().enabled = false;
    }

    //TODO SAM : damage value sync over server?

    // Whenever damage is dealt, a small graphic appears with the amount of damage dealt
    public void SpawnDamagePopUp(
        GameObject damage_object,
        Vector2 damage_target,
        int damage_value
        )
    {
        GameObject damageIndicator = Instantiate(damage_object, damage_target, Quaternion.Euler(Vector3.zero));
        damageIndicator.GetComponent<DamageValues>().damage_value = damage_value;
    }

    // spawn impact objects. If the projectile does not pierce then destroy the projectile that caused the impact
    public void SpawnImpact(
        bool use_impact,
        GameObject impact_object,
        Vector2 impact_position,
        Quaternion impact_rotation,
        bool piercing,
        GameObject projectile)
    {

        if (use_impact)
        {
            GameObject impact = Instantiate(impact_object, impact_position, impact_rotation);
        }

        if (!piercing)
        {
            Destroy(projectile);
        }
    }
}
