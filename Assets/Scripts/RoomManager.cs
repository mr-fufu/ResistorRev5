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

	[SerializeField] private Transform uiControls = null;
	[SerializeField] private Button cancelButton = null;
	[SerializeField] private Button addRoomButton = null;
	[SerializeField] private Button roomButtonPrefab = null;
	[SerializeField] private Button playButton = null;
	[SerializeField] private Transform connectingToLobbyPopUp = null;
	[Tooltip("Only used to reconnect in case of disconnect. Connection normally happens on start of Lobby scene")]
	[SerializeField] private Button connectButton = null;

	private Dictionary<RoomInfo, Button> _roomAndButtons;

	private void Start()
	{
		_roomAndButtons = new Dictionary<RoomInfo, Button>();

		connectingToLobbyPopUp.gameObject.SetActive(true);
		connectButton.gameObject.SetActive(false);
		cancelButton.gameObject.SetActive(false);
		addRoomButton.gameObject.SetActive(false);
		playButton.gameObject.SetActive(false);
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby(TypedLobby.Default);
		PhotonNetwork.AddCallbackTarget(this);

		connectingToLobbyPopUp.gameObject.SetActive(false);
		connectButton.gameObject.SetActive(false);
		addRoomButton.gameObject.SetActive(true);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		base.OnDisconnected(cause);
		
		cancelButton.gameObject.SetActive(false);
		addRoomButton.gameObject.SetActive(false);
		playButton.gameObject.SetActive(false);
		connectButton.gameObject.SetActive(true);
	}

	public void OnClick_Connect() => connectingToLobbyPopUp.gameObject.SetActive(true);
	
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
		
		cancelButton.gameObject.SetActive(true);
		addRoomButton.gameObject.SetActive(false);
		playButton.gameObject.SetActive(true);
		
		foreach (var button in _roomAndButtons.Values) { button.gameObject.SetActive(false); }
	}
	
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		//base.OnEnable();
		Debug.Log("room list update func");
		
		foreach (RoomInfo room in roomList)
		{
			//update rooms to only the open ones
			var oldRooms = roomList.Where(x => x.RemovedFromList);
			_curRoomCount = roomList.Count(x => x.IsOpen);
			if (room.IsOpen)
			{
				//RoomReceived(room);
				SpawnRoom(room);
			}
			else
			{
				RemoveOldRoomButtons(oldRooms.ToList());
			}
		}
	}

	[PunRPC]
	public void SpawnRoom(RoomInfo room)
	{
		Debug.Log("new button");
		
		Button newRoom = Instantiate(roomButtonPrefab, Vector3.zero, Quaternion.identity, uiControls);
		_roomAndButtons.Add(room, newRoom);
		newRoom.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
	}

	public override void OnLeftRoom()
	{
		base.OnEnable();
		base.OnLeftRoom();
		
		cancelButton.gameObject.SetActive(false);
		addRoomButton.gameObject.SetActive(true);
		playButton.gameObject.SetActive(false);
		
		foreach (var button in _roomAndButtons.Values) { button.gameObject.SetActive(true); }
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
				Destroy(roomAndButton);
				Debug.Log(roomAndButton.name + " is Destroyed");
			}
		}
	}
}
