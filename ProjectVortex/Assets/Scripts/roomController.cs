using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject doorExit;
    [SerializeField] private GameObject doorEnter;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private GameObject[] enemySpawnPoints;
    [SerializeField] private Collider triggerArea;

    public bool roomCleared;
    private bool roomActivated;
    private bool roomClearCheck;
    public int enemyCount;

    void Start()
    {
        // Adds this room to the goal count
        GameManager.instance.UpdateGameGoal(1);

        roomClearCheck = false;

        // Start with both doors unlocked
        SetDoorsLocked(false);

    }

    void Update()
    {
        if (roomCleared && !roomClearCheck)
        {
            roomClearCheck = true;
            UnlockExit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (roomActivated)
            return;

        if (other.CompareTag("Player"))
        {
            roomActivated = true;
            LockEntry();
         
             spawnEnemies();

            // Optional: close exit until room cleared (that way it checks if it's the last room)
            if (doorExit != null)
                doorExit.SetActive(true);
        }
    }

void spawnEnemies()
    {
        int spawnCount = 0;
        if (enemies.Count <= 0)
        {
            roomCleared = true;
        }
        foreach (var enemy in enemies)
        {
            GameObject newEnemy = Instantiate(enemy, enemySpawnPoints[spawnCount].transform.position, enemySpawnPoints[spawnCount].transform.rotation);
            newEnemy.GetComponent<enemyRoomTracker>().thisRoom = this;
            spawnCount++;
            enemyCount++;
            if (spawnCount == enemySpawnPoints.Length)
            {
                return;
            }

        }
    }

    void LockEntry()
    {
        if (doorEnter != null)
            doorEnter.SetActive(true);
    }

    void UnlockExit()
    {
        if (doorExit != null)
            doorExit.SetActive(false);

        // Decrement goal when cleared
        GameManager.instance.UpdateGameGoal(-1);
    }

    void SetDoorsLocked(bool locked)
    {
        if (doorEnter != null)
            doorEnter.SetActive(locked);
        if (doorExit != null)
            doorExit.SetActive(locked);
    }


}
