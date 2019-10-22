using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyAfterTime: NetworkBehaviour {

    public float LifeTime;
    public GameObject player;
    public bool impact;
    public bool non_networked_destroy;

    // Use this for initialization
    void Start()
    {
        if (!non_networked_destroy)
        {
            if (isServer)
            {
                player = GameObject.Find("Player1");
            }
            else if (!isServer)
            {
                player = GameObject.Find("Player2");
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            if (!non_networked_destroy)
            {

                if (impact)
                {
                    player.GetComponent<PlayerSpawnScript>().CmdSpawnImpact(impact, GetComponent<Projectile>().impact_object, GetComponent<Projectile>().impact_point.position, GetComponent<Projectile>().impact_point.rotation, false, gameObject);
                }

                NetworkServer.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}

    public void destroy_trigger()
    {
        NetworkServer.Destroy(gameObject);
    }
}
