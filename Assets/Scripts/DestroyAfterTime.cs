using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour 
{
    public float LifeTime;
    public GameObject player;
    public bool impact;
    public bool non_networked_destroy;

	void Update () {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            if (!non_networked_destroy)
            {

                if (impact)
                {
                    BattleFactorySpawn.instance.SpawnImpact(impact, GetComponent<Projectile>().impact_object, GetComponent<Projectile>().impact_point.position, GetComponent<Projectile>().impact_point.rotation, false, gameObject);
                }

                // TODO SAM: this was on network
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
