using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class getUserInfo : MonoBehaviour
{
    private PhotonView PV;
    private PhotonManager PMnager;
    Player player;
    ExitGames.Client.Photon.Hashtable Propoties = new ExitGames.Client.Photon.Hashtable();


    public PhotonAnimatorView pAnim;
    //받아오는 정보
    public string userName = "이름없슴";
    public int userSexType = 0; // 남자는 "0",여자는"1"
    //받아오는 상태정보
    public string IsWork = "false";
    public int emoIdx = 0;

    //정보 활용
    public GameObject[] chArray;
    public GameObject[] emoArray;
    public TMP_Text text;
    public Animator animator;
    private int? temp = null;

    public static Vector3 targetPos;


    private Vector3 lastPos;
    // Start is called before the first frame update


    private void OnEnable()
    {
        foreach (var ch in chArray) ch.SetActive(false); //캐릭터 타입
        foreach (var ch in emoArray) ch.SetActive(false); //캐릭터 타입



    }
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        pAnim = GetComponent<PhotonAnimatorView>();
        //pAnim.m_Animator = animator;
        PMnager = GameObject.FindAnyObjectByType<PhotonManager>();

        // 네트워크에서 자기 자신일때 자기 몸 하이드
        if (PV.IsMine)
        {
            transform.gameObject.tag = "Player";
            //Propoties = new Hashtable() { { "chType", PMnager.sexType }, { "name", PMnager.userId } };
            //PhotonNetwork.LocalPlayer.SetCustomProperties(Propoties);

            userSexType = PMnager.sexType;
            userName = PhotonNetwork.LocalPlayer.NickName;
            //userSexType = (int)PhotonNetwork.LocalPlayer.CustomProperties["chType"];
            //userSexType = (int)PV.Owner.CustomProperties["chType"];
            transform.name = userName;
            text.text = userName;

            //남자와 여자 중 하나만 활성화 -->
            if (userSexType == 0) chArray[0].SetActive(true);
            else chArray[1].SetActive(true);
            //남자와 여자 중 하나만 활성화 <---
            animator = this.transform.GetComponentInChildren<Animator>();
            pAnim.m_Animator = animator;
            for (int i = 0; i < transform.childCount; i++)//에니메이터 연결 후 자신이면 모두 하이드
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("다른 유저일 때");
            transform.gameObject.tag = "User";
            player = PV.Owner;
            userSexType = (int)player.CustomProperties["chType"];
            Debug.Log($"포톤커스텀 정보의 성별은 : {(int)player.CustomProperties["chType"]}이다");
            userName = (string)player.CustomProperties["name"];
            Debug.Log($"포톤커스텀 정보의 이름은 : {(string)player.CustomProperties["name"]}이다");
            transform.name = userName;
            text.text = userName;

            //남자와 여자 중 하나만 활성화 -->
            if (userSexType == 0) chArray[0].SetActive(true);
            else chArray[1].SetActive(true);
            //남자와 여자 중 하나만 활성화 <---
            animator = this.transform.GetComponentInChildren<Animator>();
            pAnim.m_Animator = animator;

        }

    }
    public bool isWalking;
    public bool isStoping = true;

    public float transOffset = 0.5f;//위치 변화 허용값.

    private void Update()
    {
        if (isWalking && WalkDuring > 0) WalkDuring -= 0.0025f; //워킹상태체크. 빼기로 0이 안되서 비슷한값
        if (!ComparePos() && !isWalking && isStoping)//위치가 변하였을경우 워킹상태가 아니고 멈춤상태일 경우
        {
            //Debug.Log("walk");
            isWalking = true; isStoping = false;
            WalkDuring = checkUpdateTime;
            animator.SetBool("IsMove", true);

        }
        else if (ComparePos() && !isStoping && isWalking && WalkDuring <= 0.0025)//위치가 변하지 않을경우.조금움직였을경우
        {
            //Debug.Log("stop");
            isWalking = false; isStoping = true;
            animator.SetBool("IsMove", false);
        }


        if (emoIdx != temp) //이모티콘 표시
        {
            temp = emoIdx;//바뀌었는지 확인위해 템프에 넣어서 비교

            if (emoIdx != 0 && PV.IsMine)
            {
                PV.RPC("changeEmoticon", RpcTarget.All, temp);
            }

            foreach (var item in emoArray) item.SetActive(false); //일단 모두 하이드
            for (int i = 0; i < emoArray.Length; i++)
            {

                //if (i == eventID) { emoArray[i].SetActive(true); }
                if (i == emoIdx) StartCoroutine(showEmo(i));
            }
        }
        else if (temp != null && emoIdx == 0)
        {
            foreach (var item in emoArray) item.SetActive(false); //이벤트가0이면 모두 하이드
            temp = null;
        }
    }

    [PunRPC]// 네트워크에 이모티콘 변경
    public void changeEmoticon(int num)
    {
        emoIdx = num;
    }

    IEnumerator showEmo(int idx) //이모티콘 3초간 보여주었다 하이드
    {
        if (idx > 0)
        {
            Debug.Log("이모티콘" + idx + " 초간 변경");
            emoArray[idx].SetActive(true);
            yield return new WaitForSeconds(3);
            emoArray[idx].SetActive(false);
            emoIdx = 0;
        }

    }

    public float checkUpdateTime = 0.25f;//무브를 멈출때까지 대기 시간.0.25f
    private float WalkDuring;
    bool ComparePos()//이동값이 변했는지 체크
    {
        bool isEqual = true;
        if (lastPos != transform.position)
        {
            Vector3 tempPo = lastPos - transform.position;
            float tempComp = Mathf.Abs(tempPo.x) + Mathf.Abs(tempPo.y) + Mathf.Abs(tempPo.z);
            if (tempComp < transOffset)
            {
                return isEqual;//값이 작으면 똑같은것으로 취급하여 돌려준다.
            }
            lastPos = transform.position; //값이 달라졌으니 lasPos에 달라진 값을 넣어준다
            isEqual = !isEqual;
        }

        return isEqual;
    }//위치가 변했는지 체크


}
