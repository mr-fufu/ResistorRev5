using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreCounter : MonoBehaviour {

    public Text Player_Scoring;
    public Text Enemy_Scoring;

    // TODO SAM: Sync var
    public int player_score_value;
    public int enemy_score_value;

	void Start () {
        player_score_value = 10;    
        enemy_score_value = 10;
	}

    void Update()
    {
        Player_Scoring.text = "" + player_score_value;
        Enemy_Scoring.text = "" + enemy_score_value;

        if (PhotonNetwork.IsMasterClient)
        {
            if (player_score_value == 0)
            {
                GetComponent<SceneManagementScript>().ShowEndScreen(false);
            }

            if (enemy_score_value == 0)
            {
                GetComponent<SceneManagementScript>().ShowEndScreen(true);
            }
        }
        else
        {
            if (player_score_value == 0)
            {
                GetComponent<SceneManagementScript>().ShowEndScreen(true);
            }

            if (enemy_score_value == 0)
            {
                GetComponent<SceneManagementScript>().ShowEndScreen(false);
            }
        }
    }
}
