using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UI_RoomList : MonoBehaviourPunCallbacks
{
    public List<UI_RoomSlot> UIRooms;
  
    private void Start()
    {
        foreach (UI_RoomSlot uiRoom in UIRooms)
        {
            uiRoom.gameObject.SetActive(false);
        }
    }

    // 룸 목록을 수신하는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < UIRooms.Count; ++i)
        {
            UIRooms[i].gameObject.SetActive(false);
        }
    
        int index = 0;

    
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log($"{roomInfo.Name}_({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
      
            UIRooms[index].gameObject.SetActive(true);
            UIRooms[index].Refresh(roomInfo);

            index++;
        }
    }
}