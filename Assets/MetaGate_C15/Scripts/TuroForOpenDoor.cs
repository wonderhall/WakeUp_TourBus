using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuroForOpenDoor : MonoBehaviour
{
    public bool IsOn;
    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.GetChild(0) != null)
            this.transform.GetChild(0).gameObject.SetActive(false);
        IsOn = !UserInfo.UnlockRoom[1];//유저정보 언락룸의 반대.룸을 한번이라도 열면 언락1은 투루이기 때문.
        for (int i = 0; i < UserInfo.UnlockRoom.Length; i++)
        {
            Debug.Log($"{i}번째 문의 잠금상태는 {UserInfo.UnlockRoom[i]}입니다.");
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && IsOn)
        {
            //Debug.Log("GetInTriiger");
            if (this.transform.GetChild(0) != null)
                this.transform.GetChild(0).gameObject.SetActive(true);

        }
    }

}
