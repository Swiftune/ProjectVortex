using System.Collections;
using UnityEngine;

public class SpawnCubicle : MonoBehaviour
{
    public GameObject doorHinge;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject meleeType;
    bool playerEntered;
    private void Start()
    { 
        doorHinge.transform.rotation = Quaternion.LookRotation(new Vector3(-90, 0, 0));
    }
    private void Update()
    {
        if (playerEntered)
        {
            openDoor();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }
    IEnumerator openDoor()
    {
        Quaternion open = Quaternion.LookRotation(new Vector3(180, 0, 0));
        doorHinge.transform.rotation = Quaternion.Lerp(doorHinge.transform.rotation, open, Time.deltaTime);
        yield return new WaitForSeconds(3f);
        GameObject newEnemy = Instantiate(meleeType, spawnPoint.transform.position, spawnPoint.transform.rotation);
        playerEntered = false;
    }
}
