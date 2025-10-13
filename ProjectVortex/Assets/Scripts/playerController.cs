using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [SerializeField] float HP;
    [SerializeField] float healthRegenThreshold;
    [SerializeField] float healthRegenRate;
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] float knockBackTime;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    Vector3 moveDir;
    Vector3 playerVel;
    public Vector3 knockBack;

    int jumpCount;
    float HPOrig;

    float shootTimer;

    public bool shootEnabled;
    float healthRegenTimer;
    float damageTimer;


    void Start()
    {
        HPOrig = HP;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        damageTimer += Time.deltaTime;
        if (damageTimer > healthRegenThreshold)
        {
            healthRegenTimer += Time.deltaTime;
        }

        regenHealth();
        movement();
        ShootEvent();
        sprint();
    }

    void regenHealth()
    {
        if (healthRegenTimer > healthRegenThreshold)
        {
            HP += healthRegenRate * Time.deltaTime;
            if (HP > HPOrig)
            {
                HP = HPOrig;
                healthRegenTimer = 0;
            }
        }
    }

    void movement()
    {
        knockBack = Vector3.Lerp(knockBack, Vector3.zero, Time.deltaTime * knockBackTime);

        if (controller.isGrounded)
        {
            if (playerVel.y < 0)
                playerVel.y = 0;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        Vector3 finalMove = (moveDir + knockBack) * speed + playerVel;
        controller.Move(finalMove * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    public void jump()
    {
        if (jumpCount < jumpCountMax)
        {
            if (playerVel.y < 0)
                playerVel.y = 0;

            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void ShootEvent()
    {
        if (shootEnabled && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void shoot()
    {
        shootTimer = 0;
        GameObject newBullet = Instantiate(bullet, shootPos.position, Quaternion.identity);
        newBullet.transform.rotation = Quaternion.LookRotation(shootPos.forward);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        damageTimer = 0;

        if (HP <= 0)
        {
            GameManager.instance.stateLose();
        }
    }

    public void setYVel(float amount)
    {
        playerVel.y = amount;
    }

    public int GetGrav()
    {
        return gravity;
    }

    public float GetHP()
    {
        return HP;
    }

    public float GetHPOrig()
    {
        return HPOrig;
    }

    public void applyKnockBack(Vector3 direction)
    {
        knockBack = direction;
    }
}
