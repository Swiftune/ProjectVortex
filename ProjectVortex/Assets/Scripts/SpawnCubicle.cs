using System.Collections;
using UnityEngine;

public class SpawnCubicle : MonoBehaviour
{
    public GameObject door;
    [SerializeField] GameObject spawnPoint;
    [SerializeField] GameObject meleeType;
    bool playerEntered;
    private void Update()
    {
        if(playerEntered)
        {
            openDoor();
        }
        if(door.transform.rotation.y <= -70.0f && playerEntered)
        {
            StartCoroutine(spawnEnemy());
            playerEntered = false;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }
    void openDoor()
    {
        Quaternion open = Quaternion.LookRotation(new Vector3(90, 0, 0));
        door.transform.rotation = Quaternion.Lerp(door.transform.rotation, open, Time.deltaTime);
    }
    IEnumerator spawnEnemy()
    {
        yield return new WaitForSeconds(2.5f);
        Instantiate(meleeType, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }
}
