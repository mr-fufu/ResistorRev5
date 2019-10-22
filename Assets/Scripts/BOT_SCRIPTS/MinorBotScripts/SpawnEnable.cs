using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnable : MonoBehaviour
{
    public GameObject enable_object;

    // Start is called before the first frame update
    void Start()
    {
        enable_object.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PartStats>().attached)
        {
            if (gameObject.transform.parent.transform.parent.GetComponent<StandardStatBlock>().spawned)
            {
                enable_object.SetActive(true);
            }
            else
            {
                enable_object.SetActive(false);
            }
        }
    }
}
