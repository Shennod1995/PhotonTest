using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace AnySync.Examples
{
    public class PUN2NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject _playerPrefab;
        [SerializeField]
        private Transform _startPosition;

        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions(), TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.Instantiate(_playerPrefab.name, _startPosition.position, _startPosition.rotation);
        }
    }
}