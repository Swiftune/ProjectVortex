using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer[] model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform bodyRot;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int sprintSpeed;
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
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrg;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startingPos;
    Quaternion defaultRot;
    float speedOrig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < model.Length; i++)
        {
            colorOrig = model[i].material.color;
        }
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
        speedOrig = agent.speed;
        defaultRot = bodyRot.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        cannonTimer += Time.deltaTime;
        mgTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }
        if (playerInRange && !canSeePlayer())
        {
            animate.SetFloat("Walk", agent.velocity.normalized.magnitude);
            checkRoam();
        }
        else if (!playerInRange)
        {
            animate.SetFloat("Walk", agent.velocity.normalized.magnitude);
            checkRoam();
        }
        if (runToPos())
        {
            agent.speed = sprintSpeed;
        }
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;
        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
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
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (agent.remainingDistance <= stoppingDistOrg)
                {
                    faceTarget();
                    agent.stoppingDistance = stoppingDistOrg;
                    //StartCoroutine(firingPattern());
                }
                else
                {
                    agent.speed = sprintSpeed;
                    animate.SetTrigger("Run");
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
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, Time.deltaTime * faceTargetSpeed);
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
    public void fireCannons()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
        if (cannonTimer > cannonRate)
        {
            cannonTimer = 0;
            for (int i = 0; i < cannons.Length; i++)
            {
                Instantiate(cannonRound, cannons[i].position, rot);
            }
        }

    }
    public void fireMachineGuns()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
        if (mgTimer > gunRate)
        {
            mgTimer = 0;
            for (int i = 0; i < machineGuns.Length; i++)
            {
                Instantiate(cannonRound, machineGuns[i].position, rot);
            }
        }

    }
    void fireMortar()
    {
        mortarTimer = 0;
        Quaternion archShot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y + 10, playerDir.z));
        Instantiate(explosiveShell, mortar.position, archShot);

    }
    public void takeDamage(int amount, Vector3 direction)
    {
        HP -= amount;
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
        for (int i = 0; i < model.Length; i++)
        {
            yield return new WaitForSeconds(0.1f);
            model[i].material.color = colorOrig;
        }
    }
    IEnumerator pauseForDeath()
    {
        animate.SetTrigger("Death");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    IEnumerator firingPattern()
    {
        animate.SetTrigger("CannonFire");
        yield return new WaitForSeconds(6.5f);
        animate.SetTrigger("MGFire");
    }
    void die()
    {
        StartCoroutine(pauseForDeath());
    }
    bool runToPos()
    {
        if (agent.remainingDistance > stoppingDistOrg + 1)
        {
            return true;
        }
        return false;
    }
}
