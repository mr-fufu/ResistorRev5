    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLobbyPlayer : NetworkLobbyPlayer
{
    public GameObject hosted_lobby;
    public GameObject challenge_button;
    public Text challenger_name;
    [SyncVar] public bool ready;
    public GameObject ready_indic;

    public GameObject portrait;

    public void OnClickJoin()
    {
        SendReadyToBeginMessage();
        ready = true;
    }

    public void Update()
    {
        if (hosted_lobby == null)
        {
            hosted_lobby = GameObject.FindGameObjectWithTag("HostedLobby");
        }
        else if (gameObject.transform.parent != hosted_lobby)
        {
            gameObject.transform.SetParent(hosted_lobby.transform);
        }

        if (isLocalPlayer)
        {
            if (ready)
            {
                challenge_button.SetActive(false);
                ready_indic.SetActive(true);
            }
            else
            {
                challenge_button.SetActive(true);
                ready_indic.SetActive(false);
            }
        }
        else
        {
            challenge_button.SetActive(false);
            if (ready)
            {
                ready_indic.SetActive(true);
            }
            else
            {
                ready_indic.SetActive(false);
            }
        }
    }

    // as soon as the instance enters a lobby find a gameobject with the tag "LobbyGamePool" and set this 
    // GameObject as a child. LobbyGamePool collects all lobby players and arranges games in a grid.
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        hosted_lobby = GameObject.FindGameObjectWithTag("HostedLobby");
        gameObject.transform.SetParent(hosted_lobby.transform);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            Setup();
        }
        else
        {
            OtherLobby();
        }
    }

    private void Setup()
    {
        challenger_name.text = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().match_name_input.text;
        challenge_button.SetActive(true);

        portrait.GetComponent<PortraitManager>().alt = hosted_lobby.transform.parent.gameObject.GetComponent<CustomNetworkManager>().port_alt;
        portrait.GetComponent<PortraitManager>().profile_no = hosted_lobby.transform.parent.gameObject.GetComponent<CustomNetworkManager>().port_no;
    }

    public void OtherLobby()
    {
        challenger_name.text = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>().match_name_input.text;
        challenge_button.SetActive(false);
    }
}
