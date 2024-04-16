using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowHideCorautine : MonoBehaviour
{
    public float runTime;

    public int ShowSecond;
    public int HideSecond;
    private GameObject target;

    public bool Fade;
    public float duration;
    public AnimationCurve animCurve;
    public GameObject RendererTarget;
    public Material[] materials;
    public bool isMultiMat = false;
    [Header("통일할 매터리얼")]
    public Material tempMat;

    private Color tempCol;

    [Header("ui")]
    public bool isUseText;
    public TextMeshProUGUI textMesh;

    [Header("프로그램종료 불")]
    public bool isQuit;
    public int quitTime;
    // Start is called before the first frame update
    void Start()
    {
        if (isMultiMat) tempCol = tempMat.GetColor("_Color");
        target = transform.GetChild(0).gameObject;
        target.SetActive(false);

        StartCoroutine(ShowHide(ShowSecond, HideSecond, Fade));

        if(isUseText)StartCoroutine(showMasage(ShowSecond,duration));
        if (isQuit) StartCoroutine(quitTimer(quitTime));
    }

    



    IEnumerator ShowHide(int start, int end, bool isFade)
    {
        yield return new WaitForSeconds(start);


        target.SetActive(true);
        if (isFade == true)
        {
            StartCoroutine(FadeInOut(0, 1));
            float endtime = HideSecond - duration;
            if (isMultiMat)
            {
                yield return new WaitForSeconds(duration);
                //Destroy(GameObject.Find("newObj"));
                RendererTarget.SetActive(true);
            }

            Debug.Log(endtime);
            yield return new WaitForSeconds(endtime);
            if (isMultiMat) tempMat.SetColor("_Color", tempCol);
            StartCoroutine(FadeInOut(1, 0));
            yield return new WaitForSeconds(duration);
            Destroy(GameObject.Find("newObj"));
        }
        if (isFade == true)
        {
            yield return new WaitForSeconds(duration);
            target.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(end);
            target.SetActive(false);

            if (isMultiMat) tempMat.SetColor("_Color", tempCol);
        }

    }




    //private List<Color> col = new List<Color>();
    IEnumerator FadeInOut(float fst, float fin)
    {
        #region before
        //Color col = RendererTarget.material.color;
        //Color startColor = col;
        //Color endColor = col;
        //startColor.a = fst;
        //endColor.a = fin;

        //float t = 0;
        //while (t < duration)
        //{
        //    t += Time.deltaTime;
        //    col = Color.Lerp(startColor, endColor, t / duration);
        //    Debug.Log(col);
        //    RendererTarget.material.color = col;
        //    yield return null;
        //}

        //List<Material> tempMats = new List<Material>();
        //foreach (var item in materials)
        //{

        //}
        //Material[] tempMats = new Material[materials.Length];
        //for (int i = 0; i < materials.Length; i++)
        //{
        //    tempMats[i] = materials[i];
        //} 
        #endregion
        if (isMultiMat) //매터리얼이 여러개일 때 
        {
            GameObject newobj = new GameObject();
            newobj = Instantiate(RendererTarget, RendererTarget.transform.parent);
            newobj.name = "newObj";
            SkinnedMeshRenderer rend = newobj.GetComponent<SkinnedMeshRenderer>();
            foreach (var item in rend.materials)
            {
                Destroy(item);
            }
            Debug.Log("매터리얼개수는 " + rend.materials.Length);

            //var mats = new Material[rend.materials.Length];//새로운 매터리얼그룹생성 
            //for (int i = 0; i < newobj.GetComponent<Renderer>().materials.Length; i++)
            //{
            //    mats[i] = RendererTarget.GetComponent<Renderer>().materials[0];
            //}
            //rend.materials = mats;

            RendererTarget.SetActive(false);
            //newobj.GetComponent<Renderer>().material=new Material(tempMat);



            Color col = rend.materials[0].color;
            Color startColor = rend.materials[0].color;
            Color endColor = rend.materials[0].color;
            startColor.a = fst;
            endColor.a = fin;

            //tempMat = materials[0];
            //for (int i = 0; i < materials.Length; i++)
            //{
            //    newobj.GetComponent<Renderer>().material = tempMat;
            //    yield return null;
            //}
            //materials[0]=tempMat;

            float t = 0;
            while (t < duration)
            {

                t += Time.deltaTime;
                col = Color.Lerp(startColor, endColor, t / duration);
                tempMat.color = col;
                //tempMat.SetColor("_EmissionColor", curEmColor);

                var mats = new Material[rend.materials.Length];//새로운 매터리얼그룹생성 
                for (int i = 0; i < newobj.GetComponent<Renderer>().materials.Length; i++)
                {
                    mats[i] = tempMat;
                }
                rend.materials = mats;


                yield return null;
            }
        }
        else //매터리얼이 하나일 때
        {
            Color col = RendererTarget.GetComponent<Renderer>().material.color;
            Color startColor = col;
            Color endColor = col;
            startColor.a = fst;
            endColor.a = fin;

            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                col = Color.Lerp(startColor, endColor, t / duration);
                RendererTarget.GetComponent<Renderer>().material.color = col;
                yield return null;
            }
        }

    }
    float valueToLerp;
    IEnumerator showMasage(float startTime,float duration)
    {
        textMesh.gameObject.SetActive(false);
        yield return new WaitForSeconds(startTime);
        textMesh.gameObject.SetActive(true);

        float timeStamp = Time.time;
        while (Time.time < timeStamp + duration)

        {
            valueToLerp = Mathf.Lerp(0, 1, (Time.time - timeStamp) / duration);
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, valueToLerp);

            yield return null;
        }
    }

    IEnumerator quitTimer(int quitTime)
    {
        yield return new WaitForSeconds(quitTime);
        Application.Quit();
        Debug.Log("종료하였습니다.");
    }
}
