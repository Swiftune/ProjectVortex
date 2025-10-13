using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BossEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Transform pelvisPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform shootPos4;
    [SerializeField] Transform shootPos5;
    [SerializeField] Transform shootPos6;
    [SerializeField] Transform shootPos7;
    [SerializeField] Transform shootPos8;
    [SerializeField] Transform shootPos9;
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
    Transform[] bigCannons = new Transform [4];
    Transform[] machineGuns = new Transform[4];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnsureMat();
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
        bigCannons[0] = shootPos1;
        bigCannons[1] = shootPos2;
        bigCannons[2] = shootPos3;
        bigCannons[3] = shootPos4;
        machineGuns[0] = shootPos5;
        machineGuns[1] = shootPos6;
        machineGuns[2] = shootPos7;
        machineGuns[3] = shootPos8;
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
            still();
        }
        else
        {
            run();
        }
        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
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
    void fireCannons()
    {
        shootTimer1 = 0;

    }
    void fireMachineGuns()
    {
        shootTimer2 = 0;

    }
    void fireMortar()
    {
        shootTimer3 = 0;

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
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void still()
    {
        animate.SetTrigger("Stop");
    }
    void run()
    {
        animate.SetTrigger("Run");
    }
    void shootAnim()
    {
        animate.SetTrigger("Shoot");
    }
}
