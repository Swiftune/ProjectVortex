using System.Threading;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float explodeTime;
    [SerializeField] GameObject explosion;
    float explodeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        explodeTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        explodeTimer += Time.deltaTime;

        if (explodeTimer > explodeTime)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
