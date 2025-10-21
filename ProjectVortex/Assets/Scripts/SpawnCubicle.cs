using UnityEngine;

public class SpawnCubicle : MonoBehaviour
{
    public GameObject door;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject meleeType;
    bool playerEntered;
    private void Update()
    {
        if(door.transform.rotation.x == 90)
        {
            Instantiate(meleeType, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
    void openDoor()
    {
        Quaternion open = Quaternion.LookRotation(new Vector3(90, 0, 0));
        door.transform.rotation = Quaternion.Lerp(door.transform.rotation, open, Time.deltaTime);
    }
}
