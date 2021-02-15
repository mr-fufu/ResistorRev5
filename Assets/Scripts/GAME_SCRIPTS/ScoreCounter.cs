using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class ScoreCounter : MonoBehaviourPunCallbacks {

    public Text Player_Scoring;
    public Text Enemy_Scoring;
    
    public int player_score_value;
    public int enemy_score_value;

    private SceneManagementScript sceneMgmt;

	void Start () {
        player_score_value = 10;    
        enemy_score_value = 10;

        sceneMgmt = GetComponent<SceneManagementScript>();
    }

    void Update()
    {
        Player_Scoring.text = "" + player_score_value;
        Enemy_Scoring.text = "" + enemy_score_value;

        photonView.RPC("SyncScore", RpcTarget.Others, player_score_value);
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (player_score_value == 0)
            {
                sceneMgmt.ShowEndScreen(false);
            }

            if (enemy_score_value == 0)
            {
                sceneMgmt.ShowEndScreen(true);
            }
        }
        else
        {
            if (player_score_value == 0)
            {
                sceneMgmt.ShowEndScreen(true);
            }

            if (enemy_score_value == 0)
            {
                sceneMgmt.ShowEndScreen(false);
            }
        }
    }

    [PunRPC]
    private void SyncScore(int score)
    {
        enemy_score_value = score;
    }
}
