using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnLockCheck : MonoBehaviour
{
    public bool ResetData;
    public bool[] RoomUnLock = new bool[6];
    // Start is called before the first frame update
    void Start()
    {
        if (ResetData)//√ ±‚»≠
        {
            UserInfo.UnlockRoom[0] = true;
            for (int i = 1; i < UserInfo.UnlockRoom.Length; i++)
            {
                UserInfo.UnlockRoom[i] = false;
            }
        }


    }
    public void SetRoomUnLock()
    {
        UserInfo.UnlockRoom = RoomUnLock;
    }


}
