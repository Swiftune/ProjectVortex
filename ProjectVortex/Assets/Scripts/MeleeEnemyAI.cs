using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class MeleeEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] GameObject weapon;
    [SerializeField] float attackRate;
    [SerializeField] Animator animate;

    Material mat;
    Color colorOrig;
    float attackTimer;
    float movingToPlayer;
    float angleToPlayer;
    Vector3 playerDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnsureMat();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        movingToPlayer += Time.deltaTime;
        if (RushPlayer())
        {

        }
        if(movingToPlayer != 0)
        {
            run();
        }
        else
        {
            still();
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
    bool RushPlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir, Color.greenYellow);
        RaycastHit Hit;
        if (Physics.Raycast(headPos.position, playerDir, out Hit))
        {
            if (angleToPlayer <= FOV && Hit.collider.CompareTag("Player"))
            {
                faceTarget();
                agent.SetDestination(GameManager.instance.player.transform.position);
            }
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                movingToPlayer = 0;
                if (attackTimer >= attackRate)
                {
                    attack();
                }
            }
            return true;
        }
        return false;
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void attack()
    {
        attackTimer = 0;
        attackAnim();
    }
    public void takeDamage(int amount, Vector3 direction)
    {
        HP -= amount;
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
    void attackAnim()
    {
        animate.SetTrigger("Swing");
    }
}
