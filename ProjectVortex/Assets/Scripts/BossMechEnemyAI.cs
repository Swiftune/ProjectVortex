using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer[] model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] Transform[] cannons;
    [SerializeField] Transform[] machineGuns;
    [SerializeField] Transform mortar;
    [SerializeField] GameObject cannonRound;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject explosiveShell;
    [SerializeField] float cannonRate;
    [SerializeField] float gunRate;
    [SerializeField] float mortarRate;
    [SerializeField] Animator animate;

    Color colorOrig;
    float cannonTimer;
    float mgTimer;
    float mortarTimer;
    float angleToPlayer;
    float stoppingDistOrg;
    bool playerInRange;
    Vector3 playerDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < model.Length; i++)
        {
            colorOrig = model[i].material.color;
        }
        stoppingDistOrg = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        cannonTimer += Time.deltaTime;
        mgTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;
        animate.SetFloat("Move", agent.velocity.normalized.magnitude);
        if (playerInRange && !canSeePlayer())
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir, Color.greenYellow);
        RaycastHit Hit;
        if (Physics.Raycast(headPos.position, playerDir, out Hit))
        {
            if (angleToPlayer <= FOV && Hit.collider.CompareTag("Player"))
            {
                if (agent.remainingDistance <= stoppingDistOrg)
                {
                    faceTarget();
                    agent.stoppingDistance = stoppingDistOrg;
                    if (cannonTimer > cannonRate)
                    {
                        animate.SetTrigger("CannonFire");
                        fireCannons();
                    }
                    if (mgTimer > gunRate)
                    {
                        animate.SetTrigger("MGFire");
                        fireMachineGuns();
                    }
                }
                else
                {
                    if (mortarTimer > mortarRate)
                    {
                        fireMortar();
                    }
                }
                return true;
            }
        }
        return false;

    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, Time.deltaTime);
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
            agent.stoppingDistance = 0;
        }
    }
    void fireCannons()
    {
        cannonTimer = 0;
        StartCoroutine(firingPattern2());

    }
    void fireMachineGuns()
    {
        mgTimer = 0;
        StartCoroutine(firingPattern1());
    }
    void fireMortar()
    {
        mortarTimer = 0;
        Quaternion archShot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y + 3, playerDir.z));
        Instantiate(explosiveShell, mortar.position, archShot);

    }
    public void takeDamage(int amount, Vector3 direction)
    {
        HP -= amount;
        faceTarget();
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            die();
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }
    IEnumerator flashRed()
    {
        for (int i = 0; i < model.Length; i++)
        {
            model[i].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < model.Length; i++)
        {
            model[i].material.color = colorOrig;
        }
    }
    IEnumerator pauseForDeath()
    {
        animate.SetTrigger("Death");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    IEnumerator firingPattern1()
    {
        Quaternion rotR = Quaternion.LookRotation(new Vector3(playerDir.x + 1.5f, playerDir.y, playerDir.z));
        Quaternion rotL = Quaternion.LookRotation(new Vector3(playerDir.x - 1.5f, playerDir.y, playerDir.z));
        Instantiate(bullet, machineGuns[0].position, rotR);
        Instantiate(bullet, machineGuns[1].position, rotL);
        yield return new WaitForSeconds(0.05f);
        Instantiate(bullet, machineGuns[2].position, rotR);
        Instantiate(bullet, machineGuns[3].position, rotL);
    }
    IEnumerator firingPattern2()
    {
        Quaternion rotR = Quaternion.LookRotation(new Vector3(playerDir.x + 1, playerDir.y, playerDir.z));
        Quaternion rotL = Quaternion.LookRotation(new Vector3(playerDir.x - 1, playerDir.y, playerDir.z));
        Instantiate(cannonRound, cannons[0].position, rotR);
        Instantiate(cannonRound, cannons[1].position, rotL);
        yield return new WaitForSeconds(0.05f);          
        Instantiate(cannonRound, cannons[2].position, rotR);
        Instantiate(cannonRound, cannons[3].position, rotL);
    }
    void die()
    {
        StartCoroutine(pauseForDeath());
    }
}
