using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStat : MonoBehaviour
{
    private Player _player;
    
    public Slider HealthBarSlider;
    public Slider StaminaBarSlider;

    public void SetPlayer(Player player)
    {
        _player = player;
    }
    
    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        StaminaBarSlider.value = _player.GetAbility<PlayerStemina>().StaminaPercentage;
    }
}
