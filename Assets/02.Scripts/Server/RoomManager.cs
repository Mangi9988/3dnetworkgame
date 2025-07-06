using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private Room _room;
    public Room Room => _room;
    
    private static RoomManager _instance;
    public static RoomManager Instance => _instance;

    public event Action OnRoomDataChanged;
    public event Action<string> OnPlayerEntered;
    public event Action<string> OnPlayerExited;
    public event Action<string, string> OnPlayerDeathed;

    private void Awake()
    {
        _instance = this;
    }
    
    // 내가 방에 입장하면 자동으로 호출되는 함수
    bool _initialized = false;
    public override void OnJoinedRoom()
    {
        Init();
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            Init();
        }
    }
    
    private void Init()
    {
        if (_initialized)
        {
            return;
        }
        
        _initialized = true;
        
        // 1. 플레이어 생성
        GeneratePlayer();

        // 2. 룸 설정
        SetRoom();
        
        OnRoomDataChanged?.Invoke();
    }
    
    // 새로운 플레이어가 방에 입장하면 자동으로 호출되는 함수
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        OnRoomDataChanged?.Invoke();
        // 1. 요거는 manger가 UI에 대한 의존성이 생긴다.
        
        // 2. UI가 직접 서버 로직을 아는 것은 스마트UI
        
        // 3. 결국 관리는 Manager가...
        OnPlayerEntered?.Invoke(newPlayer.NickName + "_" + newPlayer.ActorNumber);
    }
    // 플레이어가 방에서 퇴장하면 자동으로 호출되는 함수
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        OnRoomDataChanged?.Invoke();
        OnPlayerExited?.Invoke(otherPlayer.NickName + "_" + otherPlayer.ActorNumber);
    }
    
    public void OnPlayerDeath(int actorNumber, int otherActorNumber)
    {
        // actorNumber가 otherActorNumber에 의해 죽었다.
        string deathedNickname = _room.Players[actorNumber].NickName + "_" + actorNumber;
        string attackerNickname = _room.Players[otherActorNumber].NickName + "_" + otherActorNumber;
    
        OnPlayerDeathed?.Invoke(deathedNickname, attackerNickname);
    }

    private void GeneratePlayer()
    {
        // 방에 입장 완료가되면 플레이어를 생성한다.
        // 포톤에서는 게임 오브젝트 생성후 포톤 서버에 등록까지해야 한다.
        Vector3 randomPosition = SpawnPoint.Instance.GetRandomSpawnPoint();
        PhotonNetwork.Instantiate($"{LobbyScene.CharacterType}_Player", randomPosition, Quaternion.identity);
    }

    private void SetRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        Debug.Log(_room.Name);
        Debug.Log(_room.PlayerCount);
        Debug.Log(_room.MaxPlayers); 
    }
    
}