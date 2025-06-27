using TMPro;
using UnityEngine;

public class PlayerNickName : PlayerAbility
{
    public TextMeshProUGUI NicknameTextUI;

    private void Start()
    {
        NicknameTextUI.text = $"{_photonView.Owner.NickName}_{_photonView.Owner.ActorNumber}";

        if (_photonView.IsMine)
        {
            NicknameTextUI.color = Color.green;
        }
        else
        {
            NicknameTextUI.color = Color.red;
        }
    }
}
