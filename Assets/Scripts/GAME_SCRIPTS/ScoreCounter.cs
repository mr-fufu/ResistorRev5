using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;

public class ScoreCounter : MonoBehaviour {

    public Text Player_Scoring;
    public Text Enemy_Scoring;
    
    [SerializeField]
    public int player_score_value;
    [SerializeField]
    public int enemy_score_value;

    private SceneManagementScript sceneMgmt;

    [PunRPC]
    public void SyncScore(bool EnemyScored)
    {
        if (EnemyScored)
        {
            player_score_value -= 1;
            player_score_value = Mathf.Max(player_score_value, 0);
        }
        else
        {
            enemy_score_value -= 1;
            enemy_score_value = Mathf.Max(enemy_score_value, 0);
        }

        ChangeScore();
    }

    void Start () {
        player_score_value = 10;    
        enemy_score_value = 10;

        sceneMgmt = GetComponent<SceneManagementScript>();
        
        Player_Scoring.text = "" + player_score_value;
        Enemy_Scoring.text = "" + enemy_score_value;
    }

    public void ChangeScore()
    {
        Player_Scoring.text = "" + player_score_value;
        Enemy_Scoring.text = "" + enemy_score_value;

        if (PhotonNetwork.IsMasterClient)
        {
            if (player_score_value <= 0)
            {
                sceneMgmt.ShowEndScreen(false);
            }

            if (enemy_score_value <= 0)
            {
                sceneMgmt.ShowEndScreen(true);
            }
        }
        else
        {
            if (player_score_value <= 0)
            {
                sceneMgmt.ShowEndScreen(true);
            }

            if (enemy_score_value <= 0)
            {
                sceneMgmt.ShowEndScreen(false);
            }
        }
    }
}
