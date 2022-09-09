using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IControllable, IStatus
{
    private Vector3 inputDir = Vector3.zero;

    public Vector3 KeyboardInputDir
    {
        set
        {
            inputDir = value;
        }
    }

    private Vector2 mouseDelta = Vector2.zero;

    public Vector2 MouseDelta
    {
        set
        {
            mouseDelta = value;
        }
    }

    public IControllable.InputActionDelegate OnAttack { get; set; }
    public IControllable.InputActionDelegate OnRolling { get; set; }
    public IControllable.InputActionDelegate OnJump { get; set; }
    public IControllable.InputActionDelegate OnInventory { get; set; }
    public IControllable.InputActionDelegate OnPause { get; set; }

    public float HP
    {
        get => curHealth;
        set
        {
            curHealth = Mathf.Clamp( value, 0, maxHealth);
            OnHealthChange?.Invoke();
        }

    }
    public float MaxHP { get => maxHealth; }

    public float Level
    {
        get => playerLevel;
        set
        {
            playerLevel = Mathf.Clamp(value, 1, 99);
            OnLevelChange?.Invoke();
        }

    }

    public HealthDelegate OnHealthChange { get; set; }
    public LevelDelegate OnLevelChange { get; set; }
    public ExpDelegate OnExpChange { get; set; }

    public float EXP
    {
        get => playerExp;
        set
        {
            playerExp = Mathf.Clamp(value, 0, maxExp);
            OnExpChange?.Invoke();
        }
    }

    public float MaxEXP { get => maxExp; }
   
    private Rigidbody rigid = null;
    private Camera _camera = null;
    private Animator anim = null;
    private CapsuleCollider capsuleCollider = null;
    public BoxCollider weapon = null;
    public AudioSource enemyHitClip = null;
    public AudioSource swordClip = null;
    public AudioSource footClip = null;

    // 인벤토리
    private CanvasUI canvasUI = null;

    public float playerLevel = 1.0f;
    public float curHealth = 200.0f;
    public float maxHealth = 200.0f;
    public float playerExp = 0.0f;
    public float maxExp = 100.0f;

    private bool toggleCameraRotation;
    private bool isAttack;
    private bool onRolling;
    public bool isLanding;
    public bool isGround;
    public bool isJump;

    public float moveSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpPower = 5.0f;
    private float smoothness = 10f;

    private void Awake()
    {
        canvasUI = FindObjectOfType<CanvasUI>();
        rigid = GetComponent<Rigidbody>();
        _camera = Camera.main;
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        OnAttack = Attack;
        OnRolling = Rolling;
        OnJump = Jump;
        OnInventory = canvasUI.InventoryOnOff;
        OnPause = canvasUI.PauseOnOff;
    }

    private void Update()
    {
        RefreshStatus();
        Move();
        IsGround();
        LevelUp();
    }

    private void LateUpdate()
    {
        CameraRotate();
    }

    // 플레이어 시야 확인
    private void CameraRotate()
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(playerRotate),
                                                  Time.deltaTime * smoothness);
        }
        
    }

    // 마우스에 따른 플레이어 각도
    private void MouseControl()
    {
        
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(playerRotate),
                                                  Time.deltaTime * smoothness);
    }

    // 플레이어 스테이터스 새로고침
    private void RefreshStatus()
    {
        HP = curHealth;
        Level = playerLevel;
        EXP = playerExp;

    }

    // 플레이어 이동
    private void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 moveDirection = forward * inputDir.y + right * inputDir.x;

        float aValue = Keyboard.current.aKey.ReadValue();
        float dValue = Keyboard.current.dKey.ReadValue();

        // 플레이어 좌 우 이동 애니메이션
        if (aValue == 0 || dValue == 0)
        {
            anim.SetBool("isLeft", false);
            anim.SetBool("isRight", false);
        }

        // 플레이어 좌 우 이동
        if (!isAttack && !isLanding)
        {
            if (aValue == 1)
            {
                anim.SetBool("isLeft", true);
                transform.position += moveDirection.normalized * Time.deltaTime * moveSpeed;
            }
            else if(dValue == 1)
            {
                anim.SetBool("isRight", true);
                transform.position += moveDirection.normalized * Time.deltaTime * moveSpeed;
            }
            else
            {
                transform.position += moveDirection.normalized * Time.deltaTime * moveSpeed;
                float percont = ((Keyboard.current.leftShiftKey.isPressed) ? 1 : 0.5f) * moveDirection.magnitude;
                anim.SetFloat("Blend", percont, 0.1f, Time.deltaTime);
            }
        }
    }

    // 발소리 클립
    public void FootClip()
    {
        footClip.Play();
    }

    // 공격 시 함수
    private void Attack()
    {
        anim.SetTrigger("onAttack");
    }

    // 공격 시 collider 온 오프
    public void OnAttackCollision()
    {
        swordClip.Play();
        weapon.enabled = true;
        isAttack = true;
    }

    public void OffAttackCollision()
    {
        weapon.enabled = false;
        isAttack = false;
    }

    // 플레이어 회피
    private void Rolling()
    {
        weapon.enabled = false;
        anim.SetTrigger("onRolling");
        rigid.AddForce(transform.forward * 7, ForceMode.Impulse);
    }

    // 플레이어 착지 확인
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position + Vector3.up * capsuleCollider.bounds.extents.y,
                                    Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        Debug.DrawRay(transform.position, Vector3.down, Color.red);

        if (isGround && isJump)
        {
            isJump = false;
            anim.SetTrigger("onLanding");
        }
    }

    // 플레이어 점프
    private void Jump()
    {
        if (!isAttack && isGround)
        {
            anim.SetTrigger("onJump");
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    public void JumpCheck()
    {
        isJump = true;
    }
    public void LandingInCheck()
    {
        isLanding = true;
    }
    public void LnadingOutCheck()
    {
        isLanding = false;
    }

    // 플레이어 레벨 업
    private void LevelUp()
    {
        if (maxExp <= playerExp)
        {
            playerLevel ++;
            playerExp = 0.0f;
            maxExp *= 1.3f;
            maxHealth += 100.0f;
            curHealth = maxHealth;
        }
    }

    // 플레이어 피격 함수
    public void TakeDamage(float damage)
    {
        curHealth -= damage;

        if(curHealth <= 0.0f)
        {
            Die();
        }
        HP = curHealth;
    }

    // 플레이어 경험치 습득
    public void TakeExp(float exp)
    {
        playerExp += exp;
    }

    // 플레이어 사망
    private void Die()
    {

    }
}
