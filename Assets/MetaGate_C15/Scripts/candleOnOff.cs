using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class candleOnOff : MonoBehaviour
{

    public GameObject[] targets;
    public Material[] mats;
    public float startTime;
    public int endTime;
    public float gradientTime;
    public Animator animator;
    //private float temp_Time;
    private bool isFire;
    private bool isStart;
    private bool isEnd;

    private float fireOffTime;
    private int seconds;
    private float timer;
    // Start is called before the first frame update
    private void Awake()
    {
        //temp_Time = GameObject.Find("Candle_Root").GetComponent<EventInfo>().startSecond;
        for (int i = 0; i < targets.Length; i++) { targets[i].SetActive(false); }
        targets[0].transform.GetChild(0).GetComponent<Renderer>().material = mats[0];
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        seconds = (int)(timer % 60);

        if (seconds > startTime && !isStart)
        {
            isStart = true;
            StartCoroutine(ScreenFade(0, 1, isEnd));
            Debug.Log("시작페이드");
        }
        if (seconds > endTime && !isEnd)
        {
            isEnd = true;
            Debug.Log("종료페이드");
            StartCoroutine(ScreenFade(1, 0, isEnd));
        }
        if (isFire && seconds > fireOffTime + Random.Range(0, 1))
        {
            animator.SetBool("On", false);
            StartCoroutine(FireOff());
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        fireOffTime = other.GetComponent<triggerInfor>().FireOffTime;
        if (!isFire) StartCoroutine(FireOn());
        //else { animator.SetBool("On", false); isFire = false; }
    }

    IEnumerator FireOff()
    {
        yield return new WaitForSeconds(0.25f);
        isFire = false;
        targets[1].SetActive(false);
        targets[0].SetActive(true);
    }
    IEnumerator FireOn()
    {

        yield return new WaitForSeconds(Random.Range(0f, 0.15f));
        targets[0].SetActive(false);
        targets[1].SetActive(true);
        isFire = true;
        animator.SetBool("On", true);
        //yield return new WaitForSeconds(fireOffTime + Random.Range(0f, 0.25f));
        //animator.SetBool("On", false);
        //isFire = false;
    }
    //페이드인아웃
    public IEnumerator ScreenFade(float start, float end, bool isEnd)
    {
        targets[0].SetActive(true);
        targets[0].transform.GetChild(0).GetComponent<Renderer>().material = mats[0];
        Color col = mats[0].color;
        Color startColor = col;
        Color endColor = col;
        startColor.a = start;
        endColor.a = end;

        float t = 0;
        while (t < gradientTime)
        {
            t += Time.deltaTime;
            col = Color.Lerp(startColor, endColor, t / gradientTime);
            mats[0].color = col;
            yield return null;
        }
        yield return null;

        if (!isEnd) targets[0].transform.GetChild(0).GetComponent<Renderer>().material = mats[1];
        else for (int i = 0; i < targets.Length; i++) targets[i].SetActive(false);

    }
}
