using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] int HP;
    [SerializeField] float stopDist;
    [SerializeField] float stopTime;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject patrolStart;
    [SerializeField] float shootRate;
    [SerializeField] bool withGun;

    Color colorOrig;
    float shootTimer;
    bool playerInRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!withGun)
        {
            shootPos = null;
            bullet = null;
        }
            shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            agent.stoppingDistance = stopDist;
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (shootTimer > shootRate)
            {
                shoot();
            }
        }
        else
        {
            agent.stoppingDistance = 3;
            agent.SetDestination(patrolStart.transform.position);
        }
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
        yield return new WaitForSeconds(0.5f);
        model.material.color = colorOrig;
    }
}

