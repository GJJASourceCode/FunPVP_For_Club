using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IPunObservable
{
    public float speed;
    float hAxis;
    float vAxis;
    public float jumpForce = 30f;
    public LayerMask groundLayer;
    private bool isGrounded;
    bool wDown;
    bool iDown;
    bool fDown;
    bool jDown;
    float fireDelay;
    bool isFireReady;
    Weapon equipWeapon;
    private float meleeAttackDistance = 5.0f;

    Vector3 moveVec;
    Rigidbody rb;
    public float maxSpeed;

    Animator anim;
    GameObject nearObject;
    private bool isJump;
    public PhotonView pv;

    private Vector3 netPosition;
    private Quaternion netRotation;
    private Job job;
    private BoxCollider boxCollider;
    private bool isDied;
    private float attackDelay = 1f;
    private bool canAttack = true;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        equipWeapon = GetComponentInChildren<Weapon>();
        pv = GetComponent<PhotonView>();
        job = GetComponent<Job>();
        boxCollider = GetComponent<BoxCollider>();
        if (!pv.IsMine)
        {
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Move();
        }
        else
        {
            rb.position = Vector3.MoveTowards(rb.position, netPosition, Time.fixedDeltaTime * 10f);
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation,
                netRotation,
                Time.fixedDeltaTime * 360.0f
            );
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            HealthCheck();
            PlayerCountCheck();
            GetInput();
            HandleMouseClick();
        }
    }

    public void AttackMelee()
    {
        if (!pv.IsMine || equipWeapon.type != Weapon.Type.Melee)
            return;

        var hits = Physics.SphereCastAll(
            transform.position,
            2.0f,
            transform.forward,
            meleeAttackDistance
        );
        if (TryGetComponent(out Assassin a) && !a.isPlayerVisible)
        {
            pv.RPC("UseAssassinSkill", RpcTarget.All);
        }
        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Player"))
            {
                if (hit.collider.TryGetComponent(out Shield s) && s.isSkillActive)
                {
                    return;
                }
                var player = hit.collider.gameObject.GetComponent<Player>();
                if (!player.pv.IsMine)
                {
                    player.pv.RPC("TakeDamage", RpcTarget.All, job.attack);
                    return;
                }
            }
        }
    }

    IEnumerator AttackDelayCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    [PunRPC]
    public void PlayClickAnimation()
    {
        if (equipWeapon != null && equipWeapon.type == Weapon.Type.Melee)
        {
            anim.SetTrigger("swing");
        }
        else if (equipWeapon != null && equipWeapon.type == Weapon.Type.Range)
        {
            anim.SetTrigger("shot");
        }
        else if (equipWeapon != null && equipWeapon.type == Weapon.Type.Wand)
        {
            anim.SetTrigger("castSpell");
        }
        else if (equipWeapon != null && equipWeapon.type == Weapon.Type.Dice)
        {
            anim.SetTrigger("Roll_dice");
        }
    }

    [PunRPC]
    public void Use()
    {
        GetComponentInChildren<Weapon>().Use();
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        job.hp -= damage;
        Debug.Log(job.hp);
    }

    [PunRPC]
    public void Knockback(Vector3 force)
    {
        rb.AddForce(force);
    }

    public void HealthCheck()
    {
        if (job.hp <= 0 && !isDied)
        {
            isDied = true;
            StartCoroutine(Die());
        }
    }

    public void PlayerCountCheck()
    {
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Win_Scene");
        }
    }

    [PunRPC]
    public void UseAssassinSkill()
    {
        GetComponent<Assassin>().TogglePlayerVisibility();
        GetComponent<Assassin>().ToggleEffectVisibility();
    }

    IEnumerator Die()
    {
        Debug.Log("Die");

        GetComponent<Rigidbody>().useGravity = false;

        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        anim.SetBool("isDied", true);
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lose_Scene");
        // anim.enabled = false;
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            pv.RPC("PlayClickAnimation", RpcTarget.All);
            //PlayClickAnimation();
            AttackMelee();
            StartCoroutine(AttackDelayCoroutine());
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.3f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Move()
    {
        moveVec = (transform.forward * vAxis + transform.right * hAxis).normalized;

        if (wDown)
            rb.AddForce(moveVec * speed * 60.0f * Time.fixedDeltaTime);
        else
            rb.AddForce(moveVec * speed * 5.0f * Time.fixedDeltaTime);

        Vector3 temp = Vector3.ClampMagnitude(
            new Vector3(rb.velocity.x, 10f, rb.velocity.z),
            maxSpeed
        );
        rb.velocity = new Vector3(temp.x, rb.velocity.y, temp.z);

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", wDown);
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJump = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            netPosition = (Vector3)stream.ReceiveNext();
            netRotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            netPosition += rb.velocity * lag;
        }
    }
}
