using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenText : MonoBehaviour
{
    public GameObject carry_over;

    // Start is called before the first frame update
    void Start()
    {
        carry_over = GameObject.Find("CarryOver");
        GetComponent<UnityEngine.UI.Text>().text = carry_over.GetComponent<StringCarryover>().carry_over_text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
