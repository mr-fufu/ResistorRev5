using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviourPunCallbacks
{
    public float LifeTime;
    public GameObject player;
    public bool impact;
    public bool non_networked_destroy;
    public bool enemy_check;

    [PunRPC]
    public void SyncIsEnemyForGeneric(bool isEnemy)
    {
        enemy_check = isEnemy;
    }

	void Update () {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            if (!non_networked_destroy)
            {
                if (impact)
                {
                    BattleFactorySpawn.instance.SpawnImpact(impact, GetComponent<Projectile>().impact_object, GetComponent<Projectile>().impact_point.position, 
                        GetComponent<Projectile>().impact_point.rotation, false, gameObject, GetComponent<Projectile>().enemy_check);
                }
                var proj = GetComponent<Projectile>();
                var isEnemy = proj == null ? enemy_check : proj.enemy_check;
                if (photonView.IsMine)
                {
                    Debug.Log("[DestroyTime Script] Object network destroyed : " + photonView.IsMine);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
              Destroy(gameObject);
            }
        }
	}

    public void destroy_trigger()
    {
        if (!non_networked_destroy)
        {
            if (photonView.IsMine)
            {
                Debug.Log("[DestroyTime Script] Projectile network destroyed : " + photonView.IsMine);
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
