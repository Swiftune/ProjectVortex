using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Material mat;
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
        EnsureMat();
        stoppingDistOrg = agent.stoppingDistance;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
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
        if (mat ==  null)
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
                if (shootTimer > shootRate)
                {
                    shoot();
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
    void shoot()
    {
        shootTimer = 0;

        // grabs player position
        Vector3 playerPos = GameManager.instance.player.transform.position;
        // finds the direction from the gun to the player
        Vector3 aimDir = (playerPos - shootPos.position).normalized;

        // Spawns the bullet and rotates to point at the player
        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
    }
    public void takeDamage(int amount)
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
}