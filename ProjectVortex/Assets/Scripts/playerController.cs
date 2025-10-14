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

    [SerializeField] float shootRate;
    [SerializeField] float bulletSpread;
    [SerializeField] float bulletCount;
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


    void Start()
    {
        HPOrig = HP;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        healthRegenTimer += Time.deltaTime;
       

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
        knockBack.x = Mathf.Abs(knockBack.x) < 0.01f ? 0 : knockBack.x;
        knockBack.y = Mathf.Abs(knockBack.y) < 0.01f ? 0 : knockBack.y;
        knockBack.z = Mathf.Abs(knockBack.z) < 0.01f ? 0 : knockBack.z;


        if (controller.isGrounded)
        {
            if (playerVel.y < 0)
                playerVel.y = 0;
            knockBack.y = 0;
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
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 shootDir = shootPos.forward;
            shootDir.x = Random.Range(shootDir.x - bulletSpread, shootDir.x + bulletSpread);
            shootDir.y = Random.Range(shootDir.y  - bulletSpread, shootDir.y + bulletSpread);
            shootDir.z = Random.Range(shootDir.z - bulletSpread, shootDir.z + bulletSpread);
            GameObject newBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDir));
        }
    }

    public void takeDamage(int amount, Vector3 direction)
    {
        applyKnockBack(direction);

        HP -= amount;
        healthRegenTimer = 0;

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
