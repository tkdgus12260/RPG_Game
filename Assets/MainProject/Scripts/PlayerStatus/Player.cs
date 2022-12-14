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
        get => DataManager.Inst.Player.HP;
        set
        {
            DataManager.Inst.Player.HP = Mathf.Clamp( value, 0, DataManager.Inst.Player.MaxHP);
            OnHealthChange?.Invoke();
        }

    }
    public float MaxHP { get => DataManager.Inst.Player.MaxHP; }

    public float Level
    {
        get => DataManager.Inst.Player.level;
        set
        {
            DataManager.Inst.Player.level = Mathf.Clamp(value, 1, 99);
            OnLevelChange?.Invoke();
        }

    }

    public HealthDelegate OnHealthChange { get; set; }
    public LevelDelegate OnLevelChange { get; set; }
    public ExpDelegate OnExpChange { get; set; }

    public float EXP
    {
        get => DataManager.Inst.Player.EXP;
        set
        {
            DataManager.Inst.Player.EXP = Mathf.Clamp(value, 0, DataManager.Inst.Player.MaxEXP);
            OnExpChange?.Invoke();
        }
    }

    public float MaxEXP { get => DataManager.Inst.Player.MaxEXP; }
   
    private Rigidbody rigid = null;
    private Camera _camera = null;
    private Animator anim = null;
    private CapsuleCollider capsuleCollider = null;
    public BoxCollider weapon = null;
    public AudioSource enemyHitClip = null;
    public AudioSource swordClip = null;
    public AudioSource footClip = null;
    public AudioSource deathClip = null;

    // ????????????
    public CanvasUI _canvasUI = null;

    private bool toggleCameraRotation;
    public bool isAttack;
    private bool onRolling;
    public bool isLanding;
    public bool isGround;
    public bool isJump;
    private bool isDodge;
    public bool isDeath;

    public float moveSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpPower = 5.0f;
    public float dodgePower = 7.0f;
    private float smoothness = 10f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        _camera = Camera.main;
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        OnAttack = Attack;
        OnRolling = Dodge;
        OnJump = Jump;
        OnInventory = _canvasUI.InventoryOnOff;
        OnPause = _canvasUI.PauseOnOff;

        //?????? ??? ???????????? ??????
        this.transform.position = DataManager.Inst.Player.playerPos;
    }

    private void Update()
    {
        if (!isDeath)
        {
            Move();
        }

        RefreshStatus();
        IsGround();
        LevelUp();
    }

    private void LateUpdate()
    {
        CameraRotate();
    }

    // ???????????? ?????? ??????
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

    // ???????????? ??????????????? ????????????
    private void RefreshStatus()
    {
        HP = DataManager.Inst.Player.HP;
        Level = DataManager.Inst.Player.level;
        EXP = DataManager.Inst.Player.EXP;
    }

    // ???????????? ?????? ??????
    public void SavePlayerPos()
    {
        DataManager.Inst.Player.playerPos = this.transform.position;
    }

    // ???????????? ??????
    private void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 moveDirection = forward * inputDir.y + right * inputDir.x;

        float aValue = Keyboard.current.aKey.ReadValue();
        float dValue = Keyboard.current.dKey.ReadValue();

        // ???????????? ??? ??? ?????? ???????????????
        if (aValue == 0 || dValue == 0)
        {
            anim.SetBool("isLeft", false);
            anim.SetBool("isRight", false);
        }

        // ???????????? ??? ??? ??????
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

    // ????????? ??????
    public void FootClip()
    {
        footClip.Play();
    }

    public void DeathClip()
    {
        deathClip.Play();
    }

    // ?????? ??? ??????
    private void Attack()
    {
        anim.SetTrigger("onAttack");
    }

    // ?????? ??? collider ??? ??????
    public void OnAttackCollision()
    {
        swordClip.Play();
        weapon.enabled = true;
    }
    public void OffAttackCollision()
    {
        weapon.enabled = false;
    }
    // ?????? ??? ?????? ??????
    public void IsAttackOn()
    {
        isAttack = true;
    }
    public void IsAttackOff()
    {
        isAttack = false;
    }

    // ???????????? ??????
    private void Dodge()
    {
        if (!isDodge)
        {
            weapon.enabled = false;
            anim.SetTrigger("onRolling");
            rigid.AddForce(transform.forward * dodgePower, ForceMode.Impulse);
        }
    }

    public void DodgeTrue()
    {
        isDodge = true;
    }
    public void DodgeFalse()
    {
        isDodge = false;
    }

    // ???????????? ?????? ??????
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position + Vector3.up * capsuleCollider.bounds.extents.y,
                                    Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        if (isGround && isJump)
        {
            isJump = false;
            anim.SetTrigger("onLanding");
        }
    }

    // ???????????? ??????
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

    // ???????????? ?????? ???
    private void LevelUp()
    {
        if (DataManager.Inst.Player.MaxEXP <= DataManager.Inst.Player.EXP)
        {
            DataManager.Inst.Player.level ++;
            DataManager.Inst.Player.EXP = 0.0f;
            DataManager.Inst.Player.MaxEXP *= 1.3f;
            DataManager.Inst.Player.MaxHP += 100.0f;
            DataManager.Inst.Player.HP = DataManager.Inst.Player.MaxHP;
        }
    }

    // ???????????? ?????? ??????
    public void TakeDamage(float damage)
    {
        DataManager.Inst.Player.HP -= damage;

        if(DataManager.Inst.Player.HP <= 0.0f)
        {
            StartCoroutine(Die());
        }
        HP = DataManager.Inst.Player.HP;
    }

    // ???????????? ????????? ??????
    public void TakeExp(float exp)
    {
        DataManager.Inst.Player.EXP += exp;
    }

    // ???????????? ??????
    IEnumerator Die()
    {
        isDeath = true;
        anim.SetTrigger("onDeath");
        yield return new WaitForSeconds(4.0f);
        _canvasUI.GameOver();
        yield return new WaitForSeconds(4.0f);
        _canvasUI.MenuScene();
    }
}
