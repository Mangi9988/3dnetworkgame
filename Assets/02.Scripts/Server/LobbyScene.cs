using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public enum ECharacterType
{
    Male,
    Female
}

public class LobbyScene : MonoBehaviour
{
    public TMP_InputField NickinameInputField;
    public TMP_InputField RoomNameInputField;

    public static ECharacterType CharacterType = ECharacterType.Male;
    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;

    public void OnClickMaleCharacter() => OnClickCharacterTypeButton(ECharacterType.Male);
    public void OnClickFemaleCharacter() => OnClickCharacterTypeButton(ECharacterType.Female);
    public void OnClickCharacterTypeButton(ECharacterType characterType)
    {
        // 파라미터(매개변수) vs 인자
        // parameter vs argument
        
        CharacterType = characterType;
        
        MaleCharacter.SetActive(characterType == ECharacterType.Male);
        FemaleCharacter.SetActive(characterType == ECharacterType.Female);
    }
    
    private void Start()
    {
        OnClickMaleCharacter();
    }

    
    // 방 만들기 함수를 호출
    public void OnClickMakeRoomButton()
    {
        MakeRoom();
    }

    private void MakeRoom()
    {
        string nickname = NickinameInputField.text;
        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            return;
        }
        
        // 포톤에 닉네임 등록
        PhotonNetwork.NickName = nickname;
        
        // Room 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;   
        roomOptions.IsOpen = true;     // 룸 입장 가능 여부
        roomOptions.IsVisible = true;  // 로비(채널) 룸 목록에 노출시킬지 여부
        
        // Room 생성
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

}