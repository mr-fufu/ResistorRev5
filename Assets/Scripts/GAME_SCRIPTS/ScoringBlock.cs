 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringBlock : MonoBehaviour {

    public bool enemy_check;

    public GameObject score_counter;

    private void OnTriggerEnter2D(Collider2D cross_robot)
    {
        if (!enemy_check && cross_robot.gameObject.tag == "BOT_Enemy")
        {

            if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
            {
                Destroy(cross_robot.gameObject);

                score_counter.GetComponent<ScoreCounter>().player_score_value -= 1;
            }
        }
        else if (enemy_check && cross_robot.gameObject.tag == "BOT_Player")
        {

            if (cross_robot.gameObject.GetComponent<PartStats>().part_type == "LEG")
            {
                Destroy(cross_robot.gameObject);

                score_counter.GetComponent<ScoreCounter>().enemy_score_value -= 1;
            }
        }
    }
}
