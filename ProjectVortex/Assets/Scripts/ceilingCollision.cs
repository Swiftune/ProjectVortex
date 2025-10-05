using UnityEngine;

public class ceilingCollision : MonoBehaviour
{
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] playerController playerCharacter;
    [SerializeField] float rayDist;


    bool test;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.up, rayDist, collisionLayer))
        {
            test = true;
            playerCharacter.setYVel(-5);
        }
        else
        {
            test = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == collisionLayer)
        {
            playerCharacter.GetComponentInParent<playerController>().setYVel(0);
        }

        Debug.Log("Collided!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = test ? Color.green : Color.red;

        Gizmos.DrawRay(transform.position, Vector3.up * rayDist);
    }

}

