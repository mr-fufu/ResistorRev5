using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScript : MonoBehaviour
{
    public bool flash;

    public int flash_count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flash)
        {
            flash_count++;

            if (flash_count < 8 || flash_count > 12 && flash_count < 16)
            {
                transform.GetChild(0).gameObject.SetActive(false);

            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }

            if (flash_count >= 50)
            {
                flash_count = 0;
                flash = false;
            }
        }
    }
}

