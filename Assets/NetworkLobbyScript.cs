       using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyScript : MonoBehaviour
{
    public NetworkLobbyManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<NetworkLobbyManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
