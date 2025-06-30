using System;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : PlayerAbility
{
    public Slider HealthBarSlider;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        HealthBarSlider.value = _owner.Stat.Health / _owner.Stat.MaxHealth;
    }
}
