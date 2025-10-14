using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
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

    Material mat;
    Color colorOrig;
    float shootTimer1;
    float shootTimer2;
    float shootTimer3;
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
        EnsureMat();
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
        speedOrig = agent.speed;
        defaultRot = bodyRot.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer1 += Time.deltaTime;
        shootTimer2 += Time.deltaTime;
        shootTimer3 += Time.deltaTime;
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }
        if (roamTimer != 0)
        {
            run();
        }
        else
        {
            still();
        }
        if (playerInRange && !canSeePlayer())
        {
            bodyRot.transform.rotation = defaultRot;
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
        if (canSeePlayer())
        {
            if (runToPos())
            {
                run();
            }
            else
            {
                walk();
            }
        }
    }

    bool EnsureMat()
    {
        // if the model is missing or is not on the scene, find on the enemy
        if (model == null || !model.gameObject.scene.IsValid())
        {
            // finds the enemy render
            model = GetComponentInChildren<Renderer>(true);
        }

        // Bail if can't find the model
        if (model == null)
        {
            return false;
        }

        // set the material
        if (mat == null)
        {
            mat = model.material;
            colorOrig = mat.color;
        }

        return true;
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
                    if(shootTimer2 > gunRate)
                    {
                        fireMachineGuns();
                        shootTimer2 = 0;
                        shootTimer1 = cannonRate - 5;
                    }
                    if(shootTimer1 > cannonRate)
                    {
                        fireCannons();
                        shootTimer1 = 0;
                    }
                }
                else
                {
                    if (shootTimer3 > mortarRate)
                    {
                        fireMortar();
                        shootTimer3 = 0;
                    }
                }
                return true;
            }
        }
        return false;

    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, -playerDir.y, -playerDir.z));
        bodyRot.transform.rotation = Quaternion.Lerp(bodyRot.transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
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
        shootTimer1 = 0;
        for(int i = 0; i < cannons.Length; i++)
        {
            Instantiate(cannonRound, cannons[i].position, cannons[i].rotation);
        }

    }
    void fireMachineGuns()
    {
        shootTimer2 = 0;
        for (int i = 0; i < machineGuns.Length; i++)
        {
            Instantiate(cannonRound, machineGuns[i].position, machineGuns[i].rotation);
        }

    }
    void fireMortar()
    {
        shootTimer3 = 0;
        Instantiate(explosiveShell, mortar.position, mortar.rotation);

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
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
    IEnumerator pauseForDeath()
    {
        animate.SetTrigger("Death");
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    void still()
    {
        animate.SetTrigger("Stop");
    }
    void run()
    {
        agent.speed = sprintSpeed;
        animate.SetTrigger("Run");
    }

    void walk()
    {
        agent.speed = speedOrig;
        animate.SetTrigger("Walk");
    }
    void die()
    {
        StartCoroutine(pauseForDeath());
    }
    bool runToPos()
    {
        if (agent.remainingDistance > stoppingDistOrg + 8)
        {
            return true;
        }
        return false;
    }
}
