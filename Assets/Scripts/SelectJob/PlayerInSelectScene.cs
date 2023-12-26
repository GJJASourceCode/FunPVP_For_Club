using System.Collections;
using UnityEngine;

public class PlayerInSelectScene : MonoBehaviour
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
    WeaponInSelectScene equipWeapon;

    Vector3 moveVec;
    Rigidbody rb;
    public float maxSpeed;

    Animator anim;
    GameObject nearObject;
    private bool isJump;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        equipWeapon = GetComponentInChildren<WeaponInSelectScene>();
    }

    void Start()
    {
        StartCoroutine(SelectAnimation());
    }

    IEnumerator SelectAnimation()
    {
        anim.SetBool("isWalk", true);
        transform.Rotate(new Vector3(0, -90, 0));
        var targetPos = new Vector3(0, 0.1f, -40);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            var step = 10f * 0.01f; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return new WaitForSeconds(0.01f);
        }
        transform.Rotate(new Vector3(0, 90, 0));
        anim.SetBool("isWalk", false);
        if (TryGetComponent(out Shield shield))
        {
            yield return new WaitForSeconds(0.3f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, 60, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            shield.ActivateSkill();
            yield return new WaitForSeconds(2f);
            shield.DeactivateSkill();
        }
        else if (TryGetComponent(out Assassin assassin))
        {
            yield return new WaitForSeconds(0.3f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, 60, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            yield return new WaitForSeconds(0.5f);
            assassin.TogglePlayerVisibility();
            assassin.ToggleEffectVisibility();
            yield return new WaitForSeconds(2f);
            assassin.TogglePlayerVisibility();
            assassin.ToggleEffectVisibility();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            GetComponentInChildren<WeaponInSelectScene>().Use();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, 60, 0));
            PlayClickAnimation();
            GetComponentInChildren<WeaponInSelectScene>().Use();
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(new Vector3(0, -30, 0));
            PlayClickAnimation();
            GetComponentInChildren<WeaponInSelectScene>().Use();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        GetInput();
        // Move();
        // HandleMouseClick();
    }

    void PlayClickAnimation()
    {
        if (equipWeapon != null && equipWeapon.type == WeaponInSelectScene.Type.Melee)
        {
            anim.SetTrigger("swing");
        }
        else if (equipWeapon != null && equipWeapon.type == WeaponInSelectScene.Type.Range)
        {
            anim.SetTrigger("shot");
        }
        else if (equipWeapon != null && equipWeapon.type == WeaponInSelectScene.Type.Wand)
        {
            anim.SetTrigger("castSpell");
        }
        else if (equipWeapon != null && equipWeapon.type == WeaponInSelectScene.Type.Dice)
        {
            anim.SetTrigger("Roll_dice");
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickAnimation();
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        moveVec = (transform.forward * vAxis + transform.right * hAxis).normalized;

        if (wDown)
            rb.AddForce(moveVec * speed * 60.0f * Time.deltaTime);
        else
            rb.AddForce(moveVec * speed * 5.0f * Time.deltaTime);

        Vector3 temp = Vector3.ClampMagnitude(
            new Vector3(rb.velocity.x, 10f, rb.velocity.z),
            maxSpeed
        );
        rb.velocity = new Vector3(temp.x, rb.velocity.y, temp.z);

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", wDown);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2.0f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJump = true;
    }
}
