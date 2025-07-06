using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room : MonoBehaviour
{
    public TextMeshProUGUI RoomNameText;
    public TextMeshProUGUI MasterNameText;
    public TextMeshProUGUI PlayerCountText;
    
    private RoomInfo _roomInfo;
    
    public void Init(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        RoomNameText.text    = roomInfo.Name;
        MasterNameText.text  = string.Empty;
        PlayerCountText.text = $"({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }
    
    public void OnClickRoom()
    {
        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}
