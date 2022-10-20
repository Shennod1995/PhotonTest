using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Text _roomNameText;

    private string gameVersion = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    public override void OnJoinedRoom()
    {
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("error" + message);
    }

    //public void JoinOrCreateRoom()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = 4;
    //    roomOptions.IsVisible = true;



    //    PhotonNetwork.JoinOrCreateRoom("Room" + Random.Range(0, 1000).ToString(), roomOptions, TypedLobby.Default);
    //}

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text))
        {
            _roomNameInputField.text = "Room" + Random.Range(0, 1000).ToString();
        }

        PhotonNetwork.CreateRoom(_roomNameInputField.text);
        PhotonNetwork.JoinLobby();
    }

    private void Connect()
    {
        Debug.Log(PhotonNetwork.IsConnected);
     
        if (PhotonNetwork.IsConnected == false)
        {          
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
}