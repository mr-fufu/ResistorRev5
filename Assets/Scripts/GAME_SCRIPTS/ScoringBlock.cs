 using System;
 using System.Collections;
using System.Collections.Generic;
 using Photon.Pun;
 using UnityEngine;
 using UnityEngine.SocialPlatforms.Impl;
 using UnityEngine.UI;

public class ScoringBlock : MonoBehaviourPunCallbacks
{
    public bool enemy_check;

    [SerializeField]
    private PhotonView scoreCounterPhoton;

    private void OnTriggerEnter2D(Collider2D cross_robot)
    {
        if (!enemy_check && cross_robot.CompareTag("BOT_Enemy") && cross_robot.gameObject.GetComponent<PartStats>().partType == "LEG")
        {
            cross_robot.GetComponent<Plating>().DamagePlating(9999);

            scoreCounterPhoton.RPC("SyncScore", RpcTarget.All, true);
        }
        else if (enemy_check && cross_robot.CompareTag("BOT_Player") && cross_robot.gameObject.GetComponent<PartStats>().partType == "LEG")
        {
            cross_robot.GetComponent<Plating>().DamagePlating(9999);

            scoreCounterPhoton.RPC("SyncScore", RpcTarget.All, false);
        }
    }
}
