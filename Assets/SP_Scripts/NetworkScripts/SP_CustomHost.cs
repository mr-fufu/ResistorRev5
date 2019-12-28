using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class SP_CustomHost : MonoBehaviour
{
    private MatchInfoSnapshot match;
    public Text host_name;
    public GameObject portrait;

    public CustomNetworkManager lobby_manager;

    public bool trigger;

    void Start()
    {
        lobby_manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
    }

    private void Update()
    {
        if (trigger)
        {
            JoinMatch();
            trigger = false;
        }
    }

    public void SetupMatch(MatchInfoSnapshot setup_match, bool alt, int no)
    {
        match = setup_match;
        host_name.text = match.name;
    }

    public void JoinMatch()
    {
        if (lobby_manager == null)
        {
            lobby_manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        }

        lobby_manager.hosted_lobby.SetActive(true);
        lobby_manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, lobby_manager.OnMatchJoined);
        lobby_manager.matchmaker_lobby.SetActive(false);
    }
}
