using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    public int playerDamage;
    public int enemyDamage;
    public float knockBackAmount;
    public float damageRate;
    public int speed;
    public float destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing || type == damageType.stationary)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damageType.homing)
            {
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
            }
    }

private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && (type == damageType.moving || type == damageType.stationary || type == damageType.homing))
        {
            Vector3 pushBack = (other.transform.position - transform.position).normalized;
            pushBack.y = knockBackAmount / 5;

            if (other.CompareTag("Enemy"))
            {
                dmg.takeDamage(enemyDamage, (pushBack * knockBackAmount));
            } else if (other.CompareTag("Player"))
            {
                dmg.takeDamage(playerDamage, (pushBack * knockBackAmount));
            }
        }

        if (type == damageType.homing || type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(enemyDamage, Vector3.zero);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
