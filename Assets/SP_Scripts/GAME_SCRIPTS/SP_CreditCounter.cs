using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SP_CreditCounter : NetworkBehaviour
{
    [SyncVar] public int credit_value;
    public int credit_increase;
    public int credit_time;
    private int credit_clock;

    // Start is called before the first frame update
    void Start()
    {
        credit_value = 2500;
        credit_clock = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<UnityEngine.UI.Text>().text = "CREDITS: " + credit_value;

        if (isServer)
        {
            credit_clock++;
            if (credit_clock > credit_time)
            {
                credit_clock = 0;
                credit_value += credit_increase;
            }
        }
    }
}
