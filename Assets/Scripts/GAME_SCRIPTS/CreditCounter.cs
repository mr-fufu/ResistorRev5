using System;
using Photon.Pun;

public class CreditCounter : MonoBehaviourPunCallbacks
{
    public int credit_value;

    public int credit_increase;
    public int credit_time;
    private int credit_clock;
    private UnityEngine.UI.Text creditsText;

    void Awake()
    {
        credit_value = 2500;
        credit_clock = 0;
    }

    private void Start()
    {
        creditsText = GetComponent<UnityEngine.UI.Text>();
    }

    void Update()
    {
        creditsText.text = "CREDITS: " + credit_value;

        photonView.RPC("SyncCredits", RpcTarget.Others, credit_value);
        
        credit_clock++;
        if (credit_clock > credit_time)
        {
            credit_clock = 0;
            credit_value += credit_increase;
        }
        
    }
    
    [PunRPC]
    private void SyncCredits(int creditsValue)
    {
        // update the other person's credits that they sent over the network
        if (CompareTag("Enemy"))
        {
            credit_value = creditsValue;
        }
    }

}
