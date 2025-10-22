using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup gunPickup = other.GetComponent<IPickup>();

        if (gunPickup != null)
        {
            gun.ammoCur = gun.ammoMax;
            gunPickup.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
