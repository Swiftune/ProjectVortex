using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;
    float shootTimer;
    float angleToPlayer;
    bool playerInRange;
    Vector3 playerDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (playerInRange && canSeePlayer())
        {

        }
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(transform.position, playerDir, Color.greenYellow);
        RaycastHit Hit;
        if (Physics.Raycast(transform.position, playerDir, out Hit))
        {
            Debug.Log(Hit.collider.name);
            if (angleToPlayer <= FOV && Hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (shootTimer > shootRate)
                {
                    shoot();
                }
                return true;
            }
        }
        return false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.UpdateGameGoal(-1);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}

