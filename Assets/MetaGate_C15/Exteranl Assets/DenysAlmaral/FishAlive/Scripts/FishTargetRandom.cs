using FishAlive;
using System.Collections.Generic;
using UnityEngine;

public class FishTargetRandom : MonoBehaviour
{
    public float resetDelay = 4;
    public GameObject targetRoots;


    public List<GameObject> targets;
    private FishAnimation _FishAnimation;
    private float _resetTimer;
    private int randomNum;


    // Start is called before the first frame update
    void Start()
    {
        if (targetRoots != null)
        {
            for (int i = 0; i < targetRoots.transform.childCount; i++)
            {
                targets.Add(targetRoots.transform.GetChild(i).gameObject);
            }

        }

        _FishAnimation = GetComponent<FishAnimation>();
        randomNum = targets.Count;
    }

    // Update is called once per frame
    void Update()
    {
        _resetTimer += Time.deltaTime;
        if (_resetTimer > resetDelay)
        {
            _resetTimer = 0;
            _FishAnimation.Target = targets[UnityEngine.Random.Range(0, randomNum)];

        }
    }
}
