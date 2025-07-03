using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    private int _score = 0;
    public int Score => _score;
    
    private Dictionary<string, int> _scores = new Dictionary<string, int>();

    public Dictionary<string, int> Scores => _scores;

    public event Action OnDataChanged;
    
    public override void OnJoinedRoom()
    {
        // 방에 들어가면 '네 점수가 0이다' 라는 내용으로 커스텀 프로퍼티를 초기화 해준다.
        RefreshScore();
    }
    
    private void RefreshScore()
    {
        // 최초 등록
        Hashtable hashtable = new Hashtable();
        hashtable.Add("Score", _score);
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }
    
    public void AddScore(int addScore)
    {
        _score += addScore;
        
        // 프로퍼티 벨류 수정
        RefreshScore();
    }

    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 콜백 함수
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable hashtable)
    {
        // Debug.Log($"Player {targetPlayer.NickName}_{targetPlayer.ActorNumber}의 점수 : {hashtable["Score"]}");
        
        var roomPlayers = PhotonNetwork.PlayerList;

        foreach (Photon.Realtime.Player player in roomPlayers)
        {
            if (player.CustomProperties.ContainsKey("Score"))
            {
                _scores.Add($"{player.NickName}_{player.ActorNumber}", (int)player.CustomProperties["Score"]);   
            }
        }
        
        OnDataChanged?.Invoke();
    }
}
