using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject doorExit;
    [SerializeField] private GameObject doorEnter;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private Collider triggerArea;

    private bool roomCleared;
    private bool roomActivated;

    void Start()
    {
        // Adds this room to the goal count
        GameManager.instance.UpdateGameGoal(1);

        // Disable enemies until player enters
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }

        // Start with both doors unlocked
        SetDoorsLocked(false);

    }

    void Update()
    {
        if (!roomActivated || roomCleared)
            return;

        bool allDead = true;

        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            roomCleared = true;
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

            // Activate enemies when player enters
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                    enemy.SetActive(true);
            }

            // Optional: close exit until room cleared (that way it checks if there's 
            if (doorExit != null)
                doorExit.SetActive(true);
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
