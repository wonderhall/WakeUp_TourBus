using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class ControllPP : MonoBehaviour
{
    private Volume v;
    private Bloom b;
    private Vignette vg;
    private ColorAdjustments c;

    public AnimationCurve animCurve;
    public float duration = 3f;
   public float FstExposurVal = 0f;
    public float FinExposurVal = 10;

    // Start is called before the first frame update
    void Start()
    {
        v = GetComponent<Volume>();
        //v.profile.TryGet(out b);
        //v.profile.TryGet(out vg);
        v.profile.TryGet(out c);
        StartCoroutine(exposureUp(FinExposurVal));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator exposureUp(float FinV)
    {
        FstExposurVal = 0f;

        float timeStamp = Time.time;
        while (Time.time < timeStamp + duration)
        {
            float t = (Time.time - timeStamp) / duration;
            t = animCurve.Evaluate(t);

            // 0에서 10까지 에니메이션 커브를 따라 변형
            FstExposurVal = Mathf.LerpUnclamped(0f,FinV, t);
            //c.postExposure.value = FstExposurVal;
           c.colorFilter.value = Color.Lerp(Color.white, Color.black, FstExposurVal);
            

            yield return null;
        }
        FstExposurVal = FinV;
    }
}
