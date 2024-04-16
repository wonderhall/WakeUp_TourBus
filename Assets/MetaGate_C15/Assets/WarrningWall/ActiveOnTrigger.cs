using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ActiveOnTrigger : MonoBehaviour
{

    public GameObject[] GameObjectsList;



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("경고 트리거충돌");
            for (int i = 0; i < GameObjectsList.Length; ++i)
                GameObjectsList[i].SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("경고 트리거충돌");
            for (int i = 0; i < GameObjectsList.Length; ++i)
                GameObjectsList[i].SetActive(false);
        }
    }
}
