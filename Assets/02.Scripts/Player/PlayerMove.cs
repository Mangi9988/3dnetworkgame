using Photon.Pun;
using UnityEngine;

public class PlayerMove : PlayerAbility//, IPunObservable
{
    private CharacterController _characterController;
    private Animator _animator;
    private Camera _mainCamera;
    private Transform _cameraTarget;
    
    [Header("Movement")]
    [SerializeField] private float SmoothSpeedUp = 15f;
    [SerializeField] private float SmoothSpeedDown = 5f;
    private float _smoothX = 0f;
    private float _smoothZ = 0f;

    [Header("Jump")]
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.5f;
    public float FallTimeout = 0.15f;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private bool _wasGrounded;

    // private Vector3 _receivedPosition = Vector3.zero;
    // private Quaternion _receievedRotaion = Quaternion.identity;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main;
        
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        _wasGrounded = _characterController.isGrounded;
    }

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
    // stream : 서버에서 주고받을 데이터가 담겨있는 ㅂ녀수
    // info : 송수신 성공/실패 여부에 대한 로그
    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // IsWriting과 IsReading은 IsMine검사를 알아서 해주기 때문에 굳이 조건에 필요 없음
        if (stream.IsWriting)
        {   
            // 나의 데이터만 보내준다.
            // 데이터를 전송하는 상황 -> 데이터를 보내주면 되고,
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);   

        }
        else if(stream.IsReading)
        {
            // 내 데이터가 아닐때만 보내준다.
            // 데이터를 수신하는 상황 -> 받은 데이터를 세팅하면 된다.
            // 꼭 보내준 순서대로 받아야함. 아니면 에러
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receievedRotaion = (Quaternion)stream.ReceiveNext();
        }
    }*/
    
    private void Update()
    {
        if (!_photonView.IsMine)
        {
            // transform.position = Vector3.Lerp(transform.position, _receivedPosition, Time.deltaTime * 20f);
            // transform.rotation = Quaternion.Lerp(transform.rotation, _receievedRotaion, Time.deltaTime * 20f);
            
            return;
        }
        
        JumpAndGravity();
        HandlePlayerMove();
    }

    private void HandlePlayerMove()
    {
        float rawX = InputHandler.GetAxisRaw("Horizontal");
        float rawZ = InputHandler.GetAxisRaw("Vertical");

        _smoothX = (rawX > _smoothX)
            ? Mathf.MoveTowards(_smoothX, rawX, Time.deltaTime * SmoothSpeedUp)
            : Mathf.MoveTowards(_smoothX, rawX, Time.deltaTime * SmoothSpeedDown);

        _smoothZ = (rawZ > _smoothZ)
            ? Mathf.MoveTowards(_smoothZ, rawZ, Time.deltaTime * SmoothSpeedUp)
            : Mathf.MoveTowards(_smoothZ, rawZ, Time.deltaTime * SmoothSpeedDown);

        if (_animator == null)
        {
            return;
        }
        
        _animator.SetFloat("MoveX", _smoothX);
        _animator.SetFloat("MoveZ", _smoothZ); 
        _animator.SetBool("IsMove", Mathf.Abs(_smoothX) > 0f || Mathf.Abs(_smoothZ) > 0f);
        
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Attack") || stateInfo.IsTag("Skill"))
        {
            return;
        }
        
        bool isRunning = InputHandler.GetKey(KeyCode.LeftShift);
        float targetSpeed = 0f;

        if (rawX != 0f || rawZ != 0f)
        {
            if (isRunning && _owner.GetAbility<PlayerStemina>().UseDashStamina())
            {
                targetSpeed = _owner.Stat.DashSpeed;
            }
            else
            {
                _owner.GetAbility<PlayerStemina>().IsUsingStamina = false;
                targetSpeed = _owner.Stat.MoveSpeed;
            }
        }

        Vector3 inputDir = new Vector3(rawX, 0f, rawZ).normalized;
        if (inputDir != Vector3.zero)
        {
            float targetRot = (_cameraTarget != null)
                ? _cameraTarget.eulerAngles.y
                : _mainCamera.transform.eulerAngles.y;
            inputDir = Quaternion.Euler(0f, targetRot, 0f) * inputDir;
        }

        Vector3 move = inputDir * targetSpeed * Time.deltaTime;
        move.y = _verticalVelocity * Time.deltaTime;

        if (_characterController.enabled)
        {
            _characterController.Move(move);
        }
        
        Vector3 horizontalMove = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        float normalizedSpeed = Mathf.Clamp01(horizontalMove.magnitude / (_owner.Stat.MoveSpeed > 0f ? _owner.Stat.MoveSpeed : 1f));
        float animatedSpeed = Mathf.Lerp(0.3f, 1.4f, normalizedSpeed);
        _animator.SetFloat("Speed", animatedSpeed);
        
    }

    private void JumpAndGravity()
    {
        bool isGrounded = _characterController.isGrounded;
        bool justLanded = !_wasGrounded && isGrounded;
        _wasGrounded = isGrounded;

        if (isGrounded)
        {
            if (justLanded)
            {
                float hSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;
                _animator.SetBool("Grounded", true);
                _animator.SetFloat("LandBlend", (hSpeed > 0.1f) ? 1f : 0f);
            }

            _fallTimeoutDelta = FallTimeout;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (InputHandler.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_owner.Stat.JumpPower * -2f * Gravity);
                _jumpTimeoutDelta = JumpTimeout;
                _animator.SetBool("Jump", true);
            }

            if (_jumpTimeoutDelta > 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            _animator.SetBool("Grounded", true);
            _animator.SetBool("FreeFall", false);
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta > 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }

            _animator.SetBool("Grounded", false);
            _animator.SetBool("Jump", false);
            _animator.SetBool("FreeFall", _verticalVelocity < 0.0f);
        }

        if (!_characterController.isGrounded && _characterController.enabled)
        {
            _characterController.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}
