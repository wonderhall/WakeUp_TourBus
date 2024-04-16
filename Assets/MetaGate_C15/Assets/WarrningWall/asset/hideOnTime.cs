using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class hideOnTime : MonoBehaviour
{
    [SerializeField]
    private float hideTime;
    public GameObject[] GameObjectsList;

    private void Start()
    {
        StartCoroutine("hideObject");
    }


    public IEnumerator hideObject()
    {
        while (true)
        {
            yield return new WaitForSeconds(hideTime);
            for (int i = 0; i < GameObjectsList.Length; ++i)
                GameObjectsList[i].SetActive(false);


        }
    }

}
