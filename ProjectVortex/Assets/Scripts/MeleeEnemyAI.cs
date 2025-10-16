using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class MeleeEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer[] model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int FOV;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] GameObject weapon;
    [SerializeField] float attackRate;
    [SerializeField] Animator animate;

    Color colorOrig;
    float attackTimer;
    float movingToPlayer;
    float angleToPlayer;
    Vector3 playerDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < model.Length; i++)
        {
            colorOrig = model[i].material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        movingToPlayer += Time.deltaTime;
        RushPlayer();
        if(movingToPlayer != 0)
        {
            run();
        }
        else
        {
            still();
        }
    }
    void RushPlayer()
    {
        faceTarget();
        agent.SetDestination(GameManager.instance.player.transform.position);
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir, Color.greenYellow);
        RaycastHit Hit;
        if (Physics.Raycast(headPos.position, playerDir, out Hit))
        {
            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                movingToPlayer = 0;
                if (attackTimer >= attackRate)
                {
                    attack();
                }
            }
        }
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
