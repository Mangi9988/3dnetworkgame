using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerHealthBar : PlayerAbility
{
    public Slider HealthBarSlider;

    private float _receviedValue = 1f;
    
    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }
    
    private void Refresh()
    {
        if (_photonView.IsMine)
        {
            HealthBarSlider.value = _owner.Stat.Health / _owner.Stat.MaxHealth;
        }
        else
        {
            HealthBarSlider.value = _receviedValue;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_owner.Stat.Health / _owner.Stat.MaxHealth);
        }
        else if (stream.IsReading)
        {
            float value = (float)stream.ReceiveNext();
            _receviedValue = value;
        }
    }
}
