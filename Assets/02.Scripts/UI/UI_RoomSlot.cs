using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomSlot : MonoBehaviour
{
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI MasterNameText;
    public TextMeshProUGUI PlayerCountText;
    
    private RoomInfo _roomInfo;
    
    public void Refresh(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        RoomNameText.text    = roomInfo.Name;
        MasterNameText.text  = roomInfo.CustomProperties["MasterNickname"].ToString();
        PlayerCountText.text = $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }
    
    public void OnClickRoom()
    {
        LobbyScene.Instance.TryJoinRoom(_roomInfo.Name);
    }
}
