using TMPro;
using UnityEngine;

public class UI_ScoreSlot : MonoBehaviour
{
    public TextMeshProUGUI RankTextUI;
    public TextMeshProUGUI NicknameTextUI;
    public TextMeshProUGUI ScoreTextUI;
    
    public void Set(string rank, string nickname, int score)
    {
        RankTextUI.text = rank;
        NicknameTextUI.text = nickname;
        ScoreTextUI.text = score.ToString();
    }
}
