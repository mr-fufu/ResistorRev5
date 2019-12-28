 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SP_ScoringBlock : NetworkBehaviour {

    public bool enemy_check;

    public GameObject score_counter;

    private void OnTriggerEnter2D(Collider2D cross_robot)
    {
        if (isServer)
        {
            if (!enemy_check && cross_robot.gameObject.tag == "BOT_Enemy")
            {
                //NetworkServer.Destroy(cross_robot.transform.parent.parent.gameObject);

                if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
                {
                    NetworkServer.Destroy(cross_robot.gameObject);

                    //cross_robot.GetComponent<Plating>().current_plating = 0;

                    score_counter.GetComponent<ScoreCounter>().player_score_value -= 1;
                }
            }
            else if (enemy_check && cross_robot.gameObject.tag == "BOT_Player")
            {
                //NetworkServer.Destroy(cross_robot.transform.parent.parent.gameObject);

                if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
                {
                    NetworkServer.Destroy(cross_robot.gameObject);

                    //cross_robot.GetComponent<Plating>().current_plating = 0;

                    score_counter.GetComponent<ScoreCounter>().enemy_score_value -= 1;
                }
            }
        }
    }
}
