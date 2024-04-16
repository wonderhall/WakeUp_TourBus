using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInfo : MonoBehaviour
{
    public bool isDone;
    public bool isStart;
    public int startSecond;
    public int EndSecond;

    [Header("if fade use")]
    public bool isFadeIn;
    public bool isFadeOut;
    public float gradientTime = 3f;
    public new Renderer renderer = new Renderer();



}


