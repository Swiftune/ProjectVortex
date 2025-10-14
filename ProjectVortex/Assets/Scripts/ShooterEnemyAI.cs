using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ShooterEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer[] model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] float roamPauseTime;
    [SerializeField] float sprintSpeed;
    [SerializeField] float timeTillPush;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Animator animate;

    Material mat;
    Color colorOrig;
    float shootTimer;
    float roamTimer;
    float pushTimer;
    float angleToPlayer;
    float stoppingDistOrg;
    float speedOrig;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startingPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnsureMat();
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
        speedOrig = agent.speed;    
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }
        if(roamTimer != 0)
        {
            still();
        }
        else
        {
            walk();
        }
        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
        if (playerInRange && canSeePlayer())
        {
            run();
        }
    }

    bool EnsureMat()
    {
        // if the model is missing or is not on the scene, find on the enemy
        for(int i = 0; i < model.Length; i++)
        {
            if (model[i] == null || !model[i].gameObject.scene.IsValid())
            {
                // finds the enemy render
                model[i] = GetComponentInChildren<Renderer>(true);
            }

            // Bail if can't find the model
            if (model[i] == null)
            {
                return false;
            }
            // set the material
            if (mat == null)
            {
                mat = model[i].material;
                colorOrig = mat.color;
            }
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
                if (shootTimer > shootRate)
                {
                    shoot();
                }
                if (agent.remainingDistance <= stoppingDistOrg)
                {
                    faceTarget();
                    agent.stoppingDistance = stoppingDistOrg;
                }
                else
                {
                    runNgun();
                }
                if (agent.remainingDistance == 3)
                {
                    pushTimer += Time.deltaTime;
                    if (pushTimer >= timeTillPush)
                    {
                        pushTimer = 0;
                        melee();
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
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
    void shoot()
    {
        shootTimer = 0;
        shootAnim();
    }
    void runNgun()
    {
        animate.SetTrigger("Run Shoot");
    }
    void melee()
    {
        animate.SetTrigger("Melee");
    }

    public void takeDamage(int amount, Vector3 direction)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            Destroy(gameObject);
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
            yield return new WaitForSeconds(0.1f);
            model[i].material.color = colorOrig;
        }
    }

    void still()
    {
        animate.SetTrigger("Stop");
    }
    void run()
    {
        animate.SetTrigger("Run");
        agent.speed = sprintSpeed;
    }
     void walk()
    {
        animate.SetTrigger("Walk");
        agent.speed = speedOrig;
    }
    void shootAnim()
    {
        animate.SetTrigger("Shoot");
    }
}