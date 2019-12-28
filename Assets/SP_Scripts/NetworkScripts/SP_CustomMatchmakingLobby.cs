using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class SP_CustomMatchmakingLobby : MonoBehaviour
{
    public CustomNetworkManager lobby_manager;
    public GameObject matchmaking_lobby_player;
    public GameObject matchmaking_lobby;
    
    void Start()
    {
        lobby_manager = gameObject.GetComponent<CustomNetworkManager>();
    }

    public void PopulateLobby()
    {
        if (lobby_manager == null)
        {
            lobby_manager = gameObject.GetComponent<CustomNetworkManager>();
        }

        if (lobby_manager.matchMaker == null)
        {
            lobby_manager.StartMatchMaker();
        }
        lobby_manager.matchMaker.ListMatches(0, 10, "", true, 0, 0, onMatchList);
    }

    private void onMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches_list)
    {
        if (!success)
        {
            print("no matches found");
        }

        foreach (MatchInfoSnapshot match in matches_list)
        {
            GameObject hosted_game = Instantiate(matchmaking_lobby_player);
            hosted_game.transform.SetParent(matchmaking_lobby.transform);

            CustomHost host_match = hosted_game.GetComponent<CustomHost>();
            host_match.SetupMatch(match,gameObject.GetComponent<CustomNetworkManager>().port_alt, gameObject.GetComponent<CustomNetworkManager>().port_no);
        }
    }
}
