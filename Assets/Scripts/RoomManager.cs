using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
	[Tooltip("The maximum number of rooms in the game")]
	[SerializeField] private byte maxRooms = 5;
	
	[Tooltip("The maximum number of players in each room")]
	private const byte MAXPlayersPerRoom = 5;
	
	private int _curRoomCount;
	private List<RoomInfo> _oldRooms;

	[SerializeField] private Transform roomGrid = null;
	[SerializeField] private Button cancelButton = null;
	[SerializeField] private Button addRoomButton = null;
	[SerializeField] private Button roomButtonPrefab = null;
	[SerializeField] private Button playButton = null;
	[SerializeField] private Transform connectingToLobbyPopUp = null;
	[Tooltip("Only used to reconnect in case of disconnect. Connection normally happens on start of Lobby scene")]
	[SerializeField] private Button reconnectButton = null;
	[SerializeField] private Transform availableRoomsPrompt = null;
	[SerializeField] private Transform roomTitle = null;

	private Dictionary<RoomInfo, Button> _roomAndButtons;

	private void Start()
	{
		_roomAndButtons = new Dictionary<RoomInfo, Button>();
		_oldRooms = new List<RoomInfo>();

		connectingToLobbyPopUp.gameObject.SetActive(true);
		playButton.gameObject.SetActive(false);
		
		UpdateUI(false, false);
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby(TypedLobby.Default);
		PhotonNetwork.AddCallbackTarget(this);

		connectingToLobbyPopUp.gameObject.SetActive(false);
		UpdateUI(false, true);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);

		UpdateUI(false, false);
		playButton.gameObject.SetActive(false);
	}

	public void OnClick_Connect() => connectingToLobbyPopUp.gameObject.SetActive(true);

	private void UpdateUI(bool inRoom, bool isConnected = true)
	{
		//connection stuff
		reconnectButton.gameObject.SetActive(!isConnected);
		
		//room stuff
		roomTitle.gameObject.SetActive(inRoom && isConnected);
		cancelButton.gameObject.SetActive(inRoom && isConnected);
		
		//lobby stuff
		addRoomButton.gameObject.SetActive(!inRoom && isConnected);
		
		//hide room listings
		availableRoomsPrompt.gameObject.SetActive(!inRoom && isConnected);
		roomGrid.gameObject.SetActive(!inRoom && isConnected);
		foreach (var button in _roomAndButtons.Values) { button.gameObject.SetActive(!inRoom && isConnected); }
	}
	
	public void OnClick_CreateRoom()
	{
		//TODO: show message saying the max num of allowed rooms is reached
		if(_curRoomCount >= maxRooms)
			return;
		
		Debug.Log("add room button");
		
		RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = MAXPlayersPerRoom };

		string RoomName = "Room " + UnityEngine.Random.Range(0, 1000); 
		//TODO: allow them to decide room name?
		PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
	}

	private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
	{
		print("Create Room Failed: " + codeAndMessage[1]);

	}

	public override void OnCreatedRoom()
	{
		base.OnCreatedRoom();
		
		print("Room Created Successfully");
	}
	
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		//base.OnEnable();
		Debug.Log("room list update func");

		_oldRooms.Clear();

		foreach (RoomInfo room in roomList)
		{
			//update rooms to only the open ones
			_curRoomCount = roomList.Count(x => x.IsOpen);
			if (room.IsOpen)
			{
				//RoomReceived(room);
				ShowOrCreateRoomButtons(room);
			}
			else
			{
				_oldRooms.Add(room);
			}
		}
		RemoveOldRoomButtons(_oldRooms);
	}

	[PunRPC]
	public void ShowOrCreateRoomButtons(RoomInfo room)
	{
		Debug.Log("new button");

		if (_roomAndButtons.ContainsKey(room))
		{
			_roomAndButtons[room].gameObject.SetActive(true);
		}
		else
		{
			Button newRoom = Instantiate(roomButtonPrefab, Vector3.zero, Quaternion.identity, roomGrid);
			newRoom.onClick.AddListener(delegate {OnClick_JoinRoom(newRoom);});
			_roomAndButtons.Add(room, newRoom);
			newRoom.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
		}
	}

	private void OnClick_JoinRoom(Button roomButton)
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			Debug.LogError("Can click join room, yet already in a room!");
			return;
		}

		if (!_roomAndButtons.ContainsValue(roomButton))
		{
			Debug.LogError("Room not found!");
			return;
		}

		PhotonNetwork.JoinRoom(roomButton.GetComponentInChildren<TextMeshProUGUI>().text);
	}

	public override void OnLeftRoom()
	{
		base.OnEnable();
		base.OnLeftRoom();
		
		playButton.gameObject.SetActive(false);
		UpdateUI(false);
	}


	public override void OnJoinedRoom()
	{
		//TODO: show player's name
		Debug.Log("Joined a room");

		UpdateUI(true);

		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
		{
			playButton.gameObject.SetActive(true);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		
		Debug.Log("Someone else joined your room :)");
		
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
		{
			playButton.gameObject.SetActive(true);
		}
	}

	//leave the room, will call OnLeftRoom
	public void OnClick_CancelButton() => PhotonNetwork.LeaveRoom();

	[PunRPC]
	public void RemoveOldRoomButtons(List<RoomInfo> oldRooms)
	{
		for(int i = 0; i < oldRooms.Count(); i++)
		{
			if(_roomAndButtons.ContainsKey(oldRooms[i]))
			{
				//Destroy the associated button, room is closed or hidden
				var roomAndButton = _roomAndButtons[oldRooms[i]];
				_roomAndButtons.Remove(oldRooms[i]);
				Destroy(roomAndButton);
				Debug.Log(roomAndButton.name + " is Destroyed");
			}
		}
	}
}
