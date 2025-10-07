using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    float shootTimer;

    public bool shootEnabled;

    void Start()
    {
        HPOrig = HP;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        movement();
        ShootEvent();
        sprint();
    }

    void movement()
    {
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

        Vector3 finalMove = moveDir * speed + playerVel;
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

    public int GetHP()
    {
        return HP;
    }

    public int GetHPOrig()
    {
        return HPOrig;
    }
}
