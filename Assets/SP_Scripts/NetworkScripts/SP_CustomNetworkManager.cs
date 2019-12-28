using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SP_CustomNetworkManager : NetworkLobbyManager
{
    // I also have no idea how any of this works, still a WIP
    public CustomNetworkManager lobby_manager;
    public InputField match_name_input;
    public GameObject hosted_lobby;
    public GameObject matchmaker_lobby;
    public CustomMatchmakingLobby matchmaking;
    public GameObject hide_menu;
    public string player_name;

    public GameObject player_portrait;
    public Vector4[] spawn_color = new Vector4[4];
    public Vector4[] spawn_color_alt = new Vector4[4];
    public Vector4 bot_color = new Vector4();

    public bool port_alt;
    public int port_no;

    private bool in_menu;

    public void Start()
    {
        spawn_color[0] = new Vector4(1f, 0.59f, 0.3f, 1f);
        spawn_color[1] = new Vector4(0.6f, 0.85f, 0.24f, 1f);
        spawn_color[2] = new Vector4(0.1f, 0.77f, 1f, 1f);
        spawn_color[3] = new Vector4(0.34f, 1f, 0.95f, 1f);

        spawn_color_alt[0] = new Vector4(1f, 0.2f, 0.69f, 1f);
        spawn_color_alt[1] = new Vector4(1f, 0.95f, 0.38f, 1f);
        spawn_color_alt[2] = new Vector4(1f, 0.29f, 0.29f, 1f);
        spawn_color_alt[3] = new Vector4(0.85f, 0.94f, 0.97f, 1f);

        hosted_lobby.SetActive(false);
        matchmaker_lobby.SetActive(false);
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name != lobbyScene)
        {
            hide_menu.SetActive(false);
            hosted_lobby.SetActive(false);
            matchmaker_lobby.SetActive(false);
        }

        else
        {
            hide_menu.SetActive(!in_menu);
        }

        if (player_portrait != null)
        {
            port_alt = player_portrait.GetComponent<PortraitManager>().alt;
            port_no = player_portrait.GetComponent<PortraitManager>().profile_no;

            if (player_portrait.GetComponent<PortraitManager>().alt)
            {
                bot_color = spawn_color_alt[player_portrait.GetComponent<PortraitManager>().profile_no];
            }
            else
            {
                bot_color = spawn_color[player_portrait.GetComponent<PortraitManager>().profile_no];
            }
        }
    }

    public void CustomHostLobby()
    {
        lobby_manager.StartMatchMaker();
        //lobby_manager.StartHost();
        lobby_manager.matchMaker.CreateMatch(match_name_input.text, 2, true, "", "", "", 0, 0, lobby_manager.OnMatchCreate);

        hosted_lobby.SetActive(true);

        in_menu = true;
    }

    public void CustomJoinLobby()
    {
        lobby_manager.StartMatchMaker();
        //lobby_manager.StartClient();

        matchmaker_lobby.SetActive(true);

        matchmaking = lobby_manager.GetComponent<CustomMatchmakingLobby>();
        matchmaking.PopulateLobby();

        hide_menu.SetActive(false);

        in_menu = true;
    }

    public void BackToMenu(bool host)
    {
        in_menu = false;

        if (host)
        {
            hosted_lobby.SetActive(false);
            lobby_manager.matches.Clear();
        }
        else
        {
            matchmaker_lobby.SetActive(false);
            lobby_manager.StopMatchMaker();
        }
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        //Debug.Log(lobby_manager.GetComponent<CustomNetworkManager>().lobbyScene);

        if (SceneManager.GetActiveScene().name != lobby_manager.GetComponent<CustomNetworkManager>().lobbyScene)
        {
            GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

            if (player_portrait.GetComponent<PortraitManager>().alt)
            {
                player.GetComponent<PlayerSpawnScript>().bot_color = spawn_color_alt[player_portrait.GetComponent<PortraitManager>().profile_no];
            }
            else
            {
                player.GetComponent<PlayerSpawnScript>().bot_color = spawn_color[player_portrait.GetComponent<PortraitManager>().profile_no];
            }

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

            ClientScene.AddPlayer(client.connection, 0);
        }
        else
        {
            GameObject lobby_player = this.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
            if ((Object)lobby_player == (Object)null)
                lobby_player = (GameObject)Object.Instantiate((Object)this.lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, lobby_player, playerControllerId);
        }
        return;
    }

    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        if (sceneName == this.playScene)
        {
            GameObject Player = (GameObject)Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    //public void StartupHost()
    //{
    //    SetPort();
    //    NetworkManager.singleton.StartHost();
    //}

    //public void JoinGame()
    //{
    //    SetIPAddress();
    //    SetPort();
    //    NetworkManager.singleton.StartClient();
    //}

    //void SetIPAddress()
    //{
    //    string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
    //    NetworkManager.singleton.networkAddress = ipAddress;
    //}

    //void SetPort()
    //{
    //    NetworkManager.singleton.networkPort = 7777;
    //}

    //private void OnLevelWasLoaded(int level)
    //{
    //    if (level == 0)
    //    {
    //        SetupMenuSceneButtons();
    //    }
    //    else
    //    {
    //        SetupOtherSceneButtons();
    //    }
    //}

    //void SetupMenuSceneButtons()
    //{
    //    GameObject.Find("HOST").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("HOST").GetComponent<Button>().onClick.AddListener(StartupHost);

    //    GameObject.Find("JOIN").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("JOIN").GetComponent<Button>().onClick.AddListener(JoinGame);
    //}

    //void SetupOtherSceneButtons()
    //{
    //    GameObject.Find("DISCONNECT").GetComponent<Button>().onClick.RemoveAllListeners();
    //    GameObject.Find("DISCONNECT").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);

    //}
}
