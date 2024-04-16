using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class tagCheck : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {

        int index = players.FindIndex(n => n.name == other.name);//리스트 비우기 위해 인덱스 찾기
        Debug.Log("지울 인덱스는 "+index);
        Destroy(players.Find(n => n.name == other.name));//오브젝트 삭제
        players.Remove(players[index]);//인덱스 삭제 리스트 삭제가 오브젝트 삭제후 해야한다.
    }
}
