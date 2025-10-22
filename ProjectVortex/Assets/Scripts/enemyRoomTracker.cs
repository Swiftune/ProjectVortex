using UnityEngine;

public class enemyRoomTracker : MonoBehaviour
{

    public RoomController thisRoom;

    private void OnDestroy()
    {
        if (thisRoom != null)
        {
            thisRoom.enemyCount--;
            if (thisRoom.enemyCount <= 0)
            {
                thisRoom.roomCleared = true;
            }
        }
    }
}
