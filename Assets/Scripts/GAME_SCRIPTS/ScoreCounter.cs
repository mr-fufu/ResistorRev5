using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreCounter : NetworkBehaviour {

    public Text Player_Scoring;
    public Text Enemy_Scoring;

    [SyncVar] public int player_score_value;
    [SyncVar] public int enemy_score_value;

	// Use this for initialization
	void Start () {
        player_score_value = 10;    
        enemy_score_value = 10;
	}

    // Update is called once per frame
    void Update()
    {
        Player_Scoring.text = "" + player_score_value;
        Enemy_Scoring.text = "" + enemy_score_value;

        if (isServer)
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
