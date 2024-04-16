using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PraticleCurveAin : MonoBehaviour
{
    public ParticleSystem pa;
    public AnimationCurve powCurve;
    public float pow;
    public float min;
    public float max;

    private float T;

    private void Start()
    {
        pa = this.GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        T += Time.deltaTime;

        var particleMainSettings = pa.main;
        particleMainSettings.startSpeed = Mathf.Lerp(min, max, powCurve.Evaluate(T / particleMainSettings.duration));
    }

}
