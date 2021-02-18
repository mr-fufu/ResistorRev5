using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BattleFactorySpawn : MonoBehaviour
{
    private float lightning_dist;
    private Vector3 lightningPoint;
    private bool alternator;
    
    public static BattleFactorySpawn instance;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void SpawnGeneric(GameObject object_to_spawn, Vector2 spawn_location, GameObject parent_object, Quaternion object_rotation)
    {
        //GameObject spawned_object = Instantiate(object_to_spawn, spawn_location, object_rotation);
        GameObject spawned_object = PhotonNetwork.Instantiate("ParticlesAndEffects/" + object_to_spawn.name, spawn_location, object_rotation);

        spawned_object.transform.parent = parent_object.transform;
    }

    // Spawn Projectile and set all according parameters from the projectile attack script that launched the projectile

    public void SpawnProjectile(GameObject projectile, Vector2 launch_location, GameObject projectile_launcher)
    {
        var projectileAttack = projectile_launcher.GetComponent<ProjectileAttack>();
        
        //GameObject clone = Instantiate(projectile, launch_location, projectileAttack.transform.rotation);
        GameObject clone = PhotonNetwork.Instantiate("ParticlesAndEffects/" + projectile.name, 
            launch_location, projectileAttack.transform.rotation);

        // sets whether the projectile moves independently or is a parent of the launcher
        if (projectileAttack.projectile_parent)
        {
            clone.transform.parent = projectile_launcher.transform;
        }

        // sets whether the damage of the projectile is determined by the prefab or the attack script (since multiple projectile
        // attacks may use the same projectile)
        if (projectileAttack.variable_damage)
        {
            clone.GetComponent<Projectile>().damage_val = projectileAttack.variable_damage_value;
        }

        // sets whether the speed of the projectile is determined by the prefab or the attack script (similar to variable_damage)
        if (projectileAttack.variable_speed)
        {
            clone.GetComponent<Projectile>().projectile_speed = projectileAttack.variable_speed_value;
        }

        // sets which player the projectile belongs to (ENEMY on the right side of screen, Non-ENEMY on the left)
        clone.GetComponent<Projectile>().enemy_check = projectileAttack.enemy_check;

        // sets the projectile to be on the same layer as the launcher (for lane sort purposes)
        clone.GetComponent<SpriteRenderer>().sortingLayerName = projectile_launcher.GetComponent<SpriteRenderer>().sortingLayerName;

        // sets whether the projectile appears over top of bots
        if (projectileAttack.attack_over)
        {
            clone.GetComponent<SpriteRenderer>().sortingOrder = (projectile_launcher.GetComponent<SpriteRenderer>().sortingOrder + 10);
        }

        // sets whether the projectile has a variable range (lifespan)
        if (projectileAttack.variable_range == true )
        {
            if (projectileAttack.range_stat_dependent)
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = 
                    projectile_launcher.transform.parent.transform.parent.gameObject.GetComponent<StandardStatBlock>().RANGE * 0.3f * Time.deltaTime * 50;
            }
            else
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectileAttack.variable_range_value * 0.3f * Time.deltaTime * 50;
            }
        }

        // sets the projectile's height (y position)
        projectile.GetComponent<Projectile>().ground_y_position = projectileAttack.leg_component.transform.position.y;
    }

    // Projectile handles most attacks but tesla attacks were a fair bit more complicated and thus have their own spawn function.
    // Lightning consists of a unique start segment (with unique animation) a looping middle section (of variable length) and an end section (unique)

    public void SpawnLightning(GameObject lightning_object, GameObject launch_point, int lightning_damage, bool enemy_check, int power)
    {
        // spawns the lightning start object and sets damage, enemy and collider size appropriately.
        //var lightning_start_segment = (GameObject)Instantiate(lightning_object, launch_point.transform.position, launch_point.transform.rotation);
        GameObject lightning_start_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightning_object.name, 
            launch_point.transform.position, launch_point.transform.rotation);

        
        var startLightningDamage = lightning_start_segment.GetComponent<LightningDamage>();
        
        startLightningDamage.damage_val = lightning_damage;
        startLightningDamage.enemy_check = enemy_check;
        startLightningDamage.lightning_start = true;
        lightning_start_segment.GetComponent<BoxCollider2D>().size = new Vector2((power + 2) * 26, 60);
        lightning_start_segment.GetComponent<BoxCollider2D>().offset = new Vector2(13 + 13 * (power), 10);

        // using the enemy check, move the launch point forwards (to the right for non_enemy and to the left for enemy)
        lightning_dist = enemy_check ? launch_point.transform.position.x - 26 : launch_point.transform.position.x + 26;

        // depending on the lightning power, spawn a number of middle lightning segments moving down further each time.
        for (int lightning_power = 0; lightning_power < power; lightning_power++)
        {
            lightningPoint = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);
            //var lightning_middle_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
            GameObject lightning_middle_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightning_object.name, 
                lightningPoint, launch_point.transform.rotation);
            
            var midLightningDamage = lightning_middle_segment.GetComponent<LightningDamage>();
            
            midLightningDamage.damage_val = lightning_damage;
            midLightningDamage.enemy_check = enemy_check;

            lightning_middle_segment.GetComponent<Collider2D>().enabled = false;

            lightning_dist = enemy_check ? launch_point.transform.position.x - 26 : launch_point.transform.position.x + 26;

            // use an alternator so odd and even lightning middle segments are different visually
            if (alternator == false)
            {
                midLightningDamage.lightning_middle1 = true;
                alternator = true;
            }
            else
            {
                midLightningDamage.lightning_middle2 = true;
                lightning_middle_segment.GetComponent<Collider2D>().enabled = false;
                alternator = false;
            }
        }

        // spawn lightning end section
        lightningPoint = new Vector3(lightning_dist, launch_point.transform.position.y, launch_point.transform.position.z);

        //var lightning_end_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
        GameObject lightning_end_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightning_object.name, 
            lightningPoint, launch_point.transform.rotation);

        var endLightningDamage = lightning_end_segment.GetComponent<LightningDamage>();
        
        endLightningDamage.damage_val = lightning_damage;
        endLightningDamage.enemy_check = enemy_check;
        endLightningDamage.lightning_end = true;

        lightning_end_segment.GetComponent<Collider2D>().enabled = false;
    }

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
        GameObject projectile,
        bool enemy_check)
    {

        if (use_impact)
        {
            GameObject impact = Instantiate(impact_object, impact_position, impact_rotation);
            //PhotonNetwork.Instantiate("ParticlesAndEffects/" + impact_object.name, 
            //    impact_position, impact_rotation);
        }
        
        if (!piercing && enemy_check != PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(projectile);
        }
    }
}
