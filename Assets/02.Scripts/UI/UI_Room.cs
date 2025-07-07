using System.Collections.Generic;
using UnityEngine;

public class UI_Room : MonoBehaviour
{
    public List<UI_RoomSlot> UIRoomSlotList;
    
    private void Start()
    {
        LobbyScene.Instance.OnDataChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        var roomList = LobbyScene.Instance.RoomList;

        int slotCount = UIRoomSlotList.Count;

        for (int i = 0; i < slotCount; ++i)
        {
            if (i >= roomList.Count)
            {
                UIRoomSlotList[i].gameObject.SetActive(false);
            }
            else
            {
                UIRoomSlotList[i].gameObject.SetActive(true);
                UIRoomSlotList[i].Refresh(roomList[i]);
            }
        }
    }
}
