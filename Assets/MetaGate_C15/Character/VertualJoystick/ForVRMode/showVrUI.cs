using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class showVrUI : MonoBehaviour
{
    public InputActionProperty Bt_ShowVrUI;
    public GameObject ui;
    public Image ChImage;
    public Image emoText;
    public Sprite[] ChImages;
    [Header("자동적용")]
    public Sprite savedSprite;

    private void Awake()
    {
        emoText.gameObject.SetActive(false);
        
    }
    private void OnEnable()
    {
        Debug.Log("showVRUI 활성화");
        //프리팹에 저장된 캐릭터타입
        //if (PlayerPrefs.GetString("m_chType") == "male") { ChImage.sprite = ChImages[0]; } //0은남자1은여자 
        ChImage.sprite = ChImages[UserInfo.chType];  //0은남자1은여자 
        savedSprite = ChImage.sprite;

        Bt_ShowVrUI.action.performed += ShowVrUI;
        //Bt_ShowVrUI.action.Enable();
        //PlayerInfo playerInfo = GameObject.FindAnyObjectByType<PlayerInfo>();
        //if (playerInfo.UseNetWork)
        //    Bt_ShowVrUI.action.performed += ShowVrUI;
        //else
        //OnDisable();

    }
    private void OnDisable()
    {
        Bt_ShowVrUI.action.performed -= ShowVrUI;
        //Bt_ShowVrUI.action.Disable();

    }

    private bool EmoUIShow = false;
    //private bool NowchangingImage;

    private void ShowVrUI(InputAction.CallbackContext context)
    {
        Debug.Log("버튼 구독");
        if (context.action.phase == InputActionPhase.Performed) TriggerPressed();
    }
    void TriggerPressed()
    {
        EmoUIShow = !EmoUIShow;
        ui.SetActive(EmoUIShow);
    }

    public void ChangToEmo(int idx)
    {
        ChImage.sprite = ChImages[idx];
        //NowchangingImage = true;
    }
    private void Update()
    {

    }
}
