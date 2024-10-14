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
            if (SceneManager.GetActiveScene().name == "Ss_YouthCenter_In")// ���� �Ѿ�� ȣ��Ǵ� ��찡 �־ ������ �̸��� �������� ȣ��
            {
                coroutine = BackToIniValue();
                StartCoroutine(coroutine);// ����ȯ�� ���� ȣ���ϰ� �ֽ�
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
