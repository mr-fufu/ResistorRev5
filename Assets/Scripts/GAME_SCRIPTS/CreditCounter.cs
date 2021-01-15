using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditCounter : MonoBehaviour
{
    //TODO SAM: sync var
    public int credit_value;

    public int credit_increase;
    public int credit_time;
    private int credit_clock;

    void Awake()
    {
        credit_value = 2500;
        credit_clock = 0;
    }

    void Update()
    {
        GetComponent<UnityEngine.UI.Text>().text = "CREDITS: " + credit_value;

        credit_clock++;
        if (credit_clock > credit_time)
        {
            credit_clock = 0;
            credit_value += credit_increase;
        }
        
    }
}
