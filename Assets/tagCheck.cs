using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class tagCheck : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {

        int index = players.FindIndex(n => n.name == other.name);//����Ʈ ���� ���� �ε��� ã��
        Debug.Log("���� �ε����� "+index);
        Destroy(players.Find(n => n.name == other.name));//������Ʈ ����
        players.Remove(players[index]);//�ε��� ���� ����Ʈ ������ ������Ʈ ������ �ؾ��Ѵ�.
    }
}
