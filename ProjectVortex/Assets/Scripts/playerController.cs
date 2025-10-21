using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

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
    public GameObject grenade;
    public int bulletDamage;
    public float shootRate;
    public int shootSpeed;
    public int shootTime;
    public int throwRate;
    public float bulletSpread;
    public float bulletCount;
    public float kickBack;
    public bool isInfinite;
    public float knockBackTime;
    public List<gunStats> gunList;

    public Transform shootPos;

    Vector3 moveDir;
    Vector3 playerVel;
    public Vector3 knockBack;
    public int grenadeVelocity;

    int jumpCount;
    float HPOrig;
    [Range(0, 1)] int gunListPos;

    float shootTimer;
    float grenadeTimer;

    public bool shootEnabled;
    public bool grenadeEnabled;
    float healthRegenTimer;


    void Start()
    {
        HPOrig = HP;
        shootTimer = shootRate;
        grenadeTimer = throwRate;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;
        grenadeTimer += Time.deltaTime;
        healthRegenTimer += Time.deltaTime;
       

        regenHealth();
        movement();
        ShootEvent();
        GrenadeEvent();
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

        switchGun();
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
        if (controller.isGrounded || jumpCount < jumpCountMax)
        {
            if (playerVel.y < 0)
                playerVel.y = 0;

            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void ShootEvent()
    {
        if (shootEnabled && shootTimer >= shootRate && gunList.Count > 0 && (gunList[gunListPos].ammoCur > 0 || gunList[gunListPos].isInfinite))
        {
            shoot();
        }
    }

    public void GrenadeEvent()
    {
       if (grenadeEnabled && grenadeTimer >= throwRate)
        {
            throwGrenade();
        }
    }

    void throwGrenade()
    {
        grenadeTimer = 0;

        Vector3 shootDir = shootPos.forward;
        Vector3 throwPos = shootPos.position;
        throwPos.y += 1;
        GameObject newGrenade = Instantiate(grenade, throwPos, Quaternion.LookRotation(shootDir));
        newGrenade.GetComponent<Rigidbody>().linearVelocity = shootDir * grenadeVelocity;

    }

    void shoot()
    {
        gunList[gunListPos].ammoCur--;

        if (gunList[gunListPos].isInfinite && gunList[gunListPos].ammoCur <= 0)
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        }

        shootTimer = 0;
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 shootDir = shootPos.forward;
            shootDir.x = Random.Range(shootDir.x - bulletSpread, shootDir.x + bulletSpread);
            shootDir.y = Random.Range(shootDir.y  - bulletSpread, shootDir.y + bulletSpread);
            shootDir.z = Random.Range(shootDir.z - bulletSpread, shootDir.z + bulletSpread);
            GameObject newBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDir));
            newBullet.GetComponent<Damage>().enemyDamage = bulletDamage;
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
        jumpCount += 1;
    }

    public void getGunStats(gunStats gun)
    {
      if (gunList.Count > 1)
        {
            gunList.Remove(gunList[1]);
        }
        gunList.Add(gun);

        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void changeGun()
    {

        bulletDamage = gunList[gunListPos].shootDamage;
        shootRate = gunList[gunListPos].shootRate;
        shootSpeed = gunList[gunListPos].shootSpeed;
        shootTime = gunList[gunListPos].shootTime;
        bulletSpread = gunList[gunListPos].spread;
        bulletCount = gunList[gunListPos].bulletCount;
        kickBack = gunList[gunListPos].kickBack;
        bullet = gunList[gunListPos].bullet;

        GameObject gunClone = Instantiate(gunList[gunListPos].gunModel, gunPos.transform.position, gunPos.transform.rotation);
        if (GameObject.Find("Main Camera/Gun Model") != null)
        {
            Destroy(GameObject.Find("Main Camera/Gun Model"));
        }

        gunClone.name = "Gun Model";

        gunClone.transform.parent = this.transform.Find("Main Camera").transform;
    }

    void switchGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

}
