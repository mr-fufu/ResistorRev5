using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnderstandingNetwork : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckSelf()
    {
        if (isServer)
        {
            Debug.Log("Is Server!");
        }

        if (isClient)
        {
            Debug.Log("Is Client!");
        }

        if (!isServer)
        {
            Debug.Log("Is NOT Server!");
        }
        
        if (!isClient)
        {
            Debug.Log("Is NOT Client!");
        }
    }
}
