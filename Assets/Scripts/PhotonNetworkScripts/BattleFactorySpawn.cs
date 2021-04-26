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

    public void SpawnGeneric(GameObject objectToSpawn, Vector2 spawnLocation, GameObject parentObject, Quaternion objectRotation)
    {
        //GameObject spawned_object = Instantiate(object_to_spawn, spawn_location, object_rotation);
        GameObject spawned_object = PhotonNetwork.Instantiate("ParticlesAndEffects/" + objectToSpawn.name, spawnLocation, objectRotation);
        spawned_object.GetComponent<DestroyAfterTime>().enemy_check = !PhotonNetwork.IsMasterClient;
        spawned_object.GetComponent<PhotonView>().RPC("SyncIsEnemyForGeneric", RpcTarget.Others, !PhotonNetwork.IsMasterClient);
        spawned_object.transform.parent = parentObject.transform;
    }

    // Spawn Projectile and set all according parameters from the projectile attack script that launched the projectile

    public void SpawnProjectile(GameObject projectile, Vector2 launch_location, GameObject projectile_launcher)
    {
        var projectileAttack = projectile_launcher.GetComponent<ProjectileAttack>();
        
        //GameObject clone = Instantiate(projectile, launch_location, projectileAttack.transform.rotation);
        GameObject clone = PhotonNetwork.Instantiate("ParticlesAndEffects/" + projectile.name, 
            launch_location, projectileAttack.transform.rotation);

        // sets whether the projectile moves independently or is a parent of the launcher
        if (projectileAttack.projectileParent)
        {
            clone.transform.parent = transform; //projectile_launcher.transform;
        }

        // sets whether the damage of the projectile is determined by the prefab or the attack script (since multiple projectile
        // attacks may use the same projectile)
        if (projectileAttack.variableDamage)
        {
            clone.GetComponent<Projectile>().damage_val = projectileAttack.variableDamageValue;
        }

        // sets whether the speed of the projectile is determined by the prefab or the attack script (similar to variable_damage)
        if (projectileAttack.variableSpeed)
        {
            clone.GetComponent<Projectile>().projectile_speed = projectileAttack.variableSpeedValue;
        }

        // sets which player the projectile belongs to (ENEMY on the right side of screen, Non-ENEMY on the left)
        clone.GetComponent<Projectile>().enemy_check = projectileAttack.enemyCheck;
        clone.GetComponent<PhotonView>().RPC("SyncIsEnemyForProjectilesAgain", RpcTarget.All, projectileAttack.enemyCheck);

        // sets the projectile to be on the same layer as the launcher (for lane sort purposes)
        clone.GetComponent<SpriteRenderer>().sortingLayerName = projectile_launcher.GetComponent<SpriteRenderer>().sortingLayerName;

        // sets whether the projectile appears over top of bots
        if (projectileAttack.attackOver)
        {
            clone.GetComponent<SpriteRenderer>().sortingOrder = (projectile_launcher.GetComponent<SpriteRenderer>().sortingOrder + 10);
        }

        // sets whether the projectile has a variable range (lifespan)
        if (projectileAttack.variableRange == true )
        {
            if (projectileAttack.variableRangeUsesStat)
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = 
                    projectile_launcher.transform.parent.parent.GetComponent<StandardStatBlock>().RANGE * 0.3f * Time.deltaTime * 50;
            }
            else
            {
                clone.GetComponent<DestroyAfterTime>().LifeTime = projectileAttack.variableRangeValue * 0.3f * Time.deltaTime * 50;
            }
        }

        // sets the projectile's height (y position)
        projectile.GetComponent<Projectile>().ground_y_position = projectileAttack.leg_component.transform.position.y;
    }

    // Projectile handles most attacks but tesla attacks were a fair bit more complicated and thus have their own spawn function.
    // Lightning consists of a unique start segment (with unique animation) a looping middle section (of variable length) and an end section (unique)

    //----------------------------------------------------------------------------------

    public void SpawnLightning(GameObject lightningObject, GameObject launchPoint, int lightningDamage, bool enemyCheck, int power)
    {
        // spawns the lightning start object and sets damage, enemy and collider size appropriately.
        GameObject lightning_start_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightningObject.name, 
            launchPoint.transform.position, launchPoint.transform.rotation);

        var lightningView = lightning_start_segment.GetPhotonView();

        lightningView.GetComponent<PhotonView>().RPC("SyncLightning", RpcTarget.All, enemyCheck, true, false, false, false, lightningDamage, power);

        // using the enemy check, move the launch point forwards (to the right for non_enemy and to the left for enemy)
        lightning_dist = enemyCheck ? launchPoint.transform.position.x - 26 : launchPoint.transform.position.x + 26;

        //----------------------------------------------------------------------------------

        // depending on the lightning power, spawn a number of middle lightning segments moving down further each time.
        for (int lightning_power = 0; lightning_power < power; lightning_power++)
        {
            lightningPoint = new Vector3(lightning_dist, launchPoint.transform.position.y, launchPoint.transform.position.z);
            //var lightning_middle_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
            GameObject lightning_middle_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightningObject.name, 
                lightningPoint, launchPoint.transform.rotation);

            lightningView = lightning_middle_segment.GetPhotonView();

            lightningView.GetComponent<PhotonView>().RPC("SyncLightning", RpcTarget.All, enemyCheck, false, alternator, !alternator, false, lightningDamage, power);

            lightning_dist = enemyCheck ? launchPoint.transform.position.x - 26 : launchPoint.transform.position.x + 26;

            alternator = !alternator;
        }

        //----------------------------------------------------------------------------------

        // spawn lightning end section
        lightningPoint = new Vector3(lightning_dist, launchPoint.transform.position.y, launchPoint.transform.position.z);

        //var lightning_end_segment = (GameObject)Instantiate(lightning_object, lightning_point, launch_point.transform.rotation);
        GameObject lightning_end_segment = PhotonNetwork.Instantiate("ParticlesAndEffects/" + lightningObject.name, 
            lightningPoint, launchPoint.transform.rotation);

        lightningView = lightning_end_segment.GetPhotonView();

        lightningView.GetComponent<PhotonView>().RPC("SyncLightning", RpcTarget.All, enemyCheck, false, false, false, true, lightningDamage, power);
    }

    //----------------------------------------------------------------------------------

    // Whenever damage is dealt, a small graphic appears with the amount of damage dealt
    public void SpawnDamagePopUp(
        GameObject damageObject,
        Vector2 damageTarget,
        int damageValue
        )
    {
        GameObject damageIndicator = Instantiate(damageObject, damageTarget, Quaternion.Euler(Vector3.zero));
        damageIndicator.GetComponent<DamageValues>().damage_value = damageValue;
    }

    public void SpawnDamagePopUpNetwork(
    GameObject damageObject,
    Vector2 damageTarget,
    int damageValue
    )
    {
        Vector3 newTarget = new Vector3(damageTarget.x, damageTarget.y, 0);
        GameObject damageIndicator = PhotonNetwork.Instantiate("ParticlesAndEffects/" + damageObject.name, newTarget, Quaternion.Euler(Vector3.zero));
        damageIndicator.GetComponent<PhotonView>().RPC("SyncDamageValue", RpcTarget.All, damageValue);
    }

    // spawn impact objects. If the projectile does not pierce then destroy the projectile that caused the impact
    public void SpawnImpact(
        bool useImpact,
        GameObject impactObject,
        Vector2 impactPosition,
        Quaternion impactRotation,
        bool piercing,
        GameObject projectile,
        bool enemyCheck)
    {

        if (useImpact)
        {
            GameObject impact = Instantiate(impactObject, impactPosition, impactRotation);
            //PhotonNetwork.Instantiate("ParticlesAndEffects/" + impact_object.name, 
            //    impact_position, impact_rotation);
        }
        
        if (!piercing && enemyCheck != PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(projectile);
        }
    }
}
