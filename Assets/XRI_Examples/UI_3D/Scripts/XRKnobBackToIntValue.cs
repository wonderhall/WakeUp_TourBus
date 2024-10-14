using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Content.Interaction;
public class XRKnobBackToIntValue : MonoBehaviour
{
    public XRKnob XRKnob;
    public float speed = 0.1f;
    // Start is called before the first frame update

    public bool isChanging = false;
    IEnumerator coroutine;
    public void ReBackValue()
    {
        if (isChanging == false)
        {
            if (SceneManager.GetActiveScene().name == "Ss_YouthCenter_In")// 씬을 넘어가서 호출되는 경우가 있어서 지정된 이름의 씬에서만 호출
            {
                coroutine = BackToIniValue();
                StartCoroutine(coroutine);// 씬전환시 에러 호출하고 있슴
            }
        }
    }
    IEnumerator BackToIniValue()
    {
        isChanging = true;
        float v = XRKnob.value;
        while (v < 1.0f)
        {
            v += speed;
            XRKnob.value = v;
            if (v >= 1f) break;
            yield return null;
        }
        isChanging = false;
    }

    public void changIsisChanging(bool m_bool)
    {
        isChanging = m_bool;
    }
}
