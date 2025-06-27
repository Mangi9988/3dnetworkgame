using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public float MoveSpeed = 7f;
    public float DashSpeed = 10f;
    public float JumpPower = 2.5f;
    public float RotationSpeed = 300f;
    public float AttackSpeed = 1.2f;
    public float MaxStamina = 100f;
    public float RecoverStaminaAmount = 20f;
}
