using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour, IDamage, IPickup
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
    [SerializeField] GameObject gunPos;

    public GameObject bullet;
    public int bulletDamage;
    public float shootRate;
    public int shootSpeed;
    public int shootTime;
    public float bulletSpread;
    public float bulletCount;
    public float kickBack;
    public bool isInfinite;
    public float knockBackTime;

    public Transform shootPos;

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
            newBullet.GetComponent<Damage>().damageAmount = bulletDamage;
            newBullet.GetComponent<Damage>().speed = shootSpeed;
            newBullet.GetComponent<Damage>().destroyTime = shootTime;
        }
        knockBack -= shootPos.forward * kickBack;
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

    public void getGunStats(gunStats gun)
    {
        bulletDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootSpeed = gun.shootSpeed;
        shootTime = gun.shootTime;
        bulletSpread = gun.spread;
        bulletCount = gun.bulletCount;
        kickBack = gun.kickBack;
        bullet = gun.bullet;

       GameObject gunClone = Instantiate(gun.gunModel, gunPos.transform.position, gunPos.transform.rotation);
        if (GameObject.Find("Main Camera/Gun Model") != null)
        {
            Destroy(GameObject.Find("Main Camera/Gun Model"));
        }

        gunClone.name = "Gun Model";

        gunClone.transform.parent = this.transform.Find("Main Camera").transform;


}
}
