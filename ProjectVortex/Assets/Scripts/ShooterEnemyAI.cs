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
    [SerializeField] Transform[] shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Animator animate;

    Color colorOrig;
    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrg;
    bool playerInRange;
    Vector3 playerDir;
    Vector3 startingPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < model.Length; i++)
        {
            colorOrig = model[i].material.color;
        }
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        animate.SetFloat("Move", agent.velocity.normalized.magnitude);
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
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
                    shootTimer = 0;
                    animate.SetTrigger("Shoot");
                }
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
    public void shootEvent()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x -1, playerDir.y, playerDir.z));
        for (int i = 0; i < shootPos.Length; i++)
        {
            Instantiate(bullet, shootPos[i].position, rot);
        }
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
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < model.Length; i++)
        {
            model[i].material.color = colorOrig;
        }
    }
}