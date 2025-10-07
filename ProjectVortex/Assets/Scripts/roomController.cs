using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject doorExit;
    [SerializeField] private GameObject doorEnter;
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private GameObject triggerArea;

    bool roomCleared;

    void Start()
    {
        // Lock both doors at the start if there are any enemies
        if (enemies.Count > 0)
        {
            SetDoorsLocked(true);
        }
    }

    void Update()
    {
        if (roomCleared)
            return;

        bool allDead = true;

        // Check if all enemies in the list are destroyed or disabled
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && enemies[i].activeInHierarchy)
            {
                allDead = false;
                break;
            }
        }

        // Unlock doors once all enemies are gone
        if (allDead)
        {
            roomCleared = true;
            SetDoorsLocked(false);
        }
    }

    void SetDoorsLocked(bool locked)
    {
        if (doorEnter != null)
            doorEnter.SetActive(locked);

        if (doorExit != null)
            doorExit.SetActive(locked);
    }
}
