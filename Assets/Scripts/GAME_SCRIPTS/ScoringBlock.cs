 using System;
 using System.Collections;
using System.Collections.Generic;
 using Photon.Pun;
 using UnityEngine;
 using UnityEngine.SocialPlatforms.Impl;
 using UnityEngine.UI;

public class ScoringBlock : MonoBehaviour {

    public bool enemy_check;

    public GameObject score_counter;
    private ScoreCounter scoreCounter;

    private void Start()
    {
        scoreCounter = score_counter.GetComponent<ScoreCounter>();
    }

    private void OnTriggerEnter2D(Collider2D cross_robot)
    {
        if (!enemy_check && cross_robot.CompareTag("BOT_Enemy"))
        {

            if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
            {
                PhotonNetwork.Destroy(cross_robot.gameObject);

                scoreCounter.player_score_value -= 1;
                scoreCounter.ChangeScore();
                
            }
        }
        else if (enemy_check && cross_robot.CompareTag("BOT_Player"))
        {

            if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
            {
                PhotonNetwork.Destroy(cross_robot.gameObject);

                scoreCounter.enemy_score_value -= 1;
                scoreCounter.ChangeScore();
            }
        }
    }
}
