using System;
using UnityEngine;
using Photon.Pun;

public class CreditCounter : MonoBehaviourPunCallbacks
{
    public int credit_value;

    public int credit_increase;
    public float credit_time;
    private float credit_clock;
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
        credit_clock += 10.0f * Time.deltaTime;

        if (credit_clock > credit_time)
        {
            credit_clock = 0;
            credit_value += credit_increase;

            creditsText.text = "CREDITS: " + credit_value;

            photonView.RPC("SyncCredits", RpcTarget.Others, credit_value);
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
