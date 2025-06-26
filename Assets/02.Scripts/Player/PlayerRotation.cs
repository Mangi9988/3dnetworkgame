using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRotation : MonoBehaviour
{
    // 목표 : 마우스를 조작하면 캐릭터를 그 방향으로 회전시키고 싶다.
    
    public Transform CameraTarget;
    public float RotationSpeed = 10f;
    
    // 마우스 입력값을 누적할 변수
    private float _mx;
    private float _my;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        _mx += mouseX * RotationSpeed * Time.deltaTime;
        _my += mouseY * RotationSpeed * Time.deltaTime;
        
        _my = Mathf.Clamp(_my, -60f, 60f);
        
        // y축 회전은 캐릭터만 한다
        transform.eulerAngles = new Vector3(0f, _mx, 0f);
        
        // x축 회전은 캐릭터는 하지 않는다 (즉, 카메라 루트만 x축 회전하면 된다.)
        CameraTarget.localEulerAngles = new Vector3(-_my, 0f, 0f);
    }
}
