using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    ExitGames.Client.Photon.Hashtable Propoties = new ExitGames.Client.Photon.Hashtable();
    public bool IsOffLineMode;
    public string userId = "Tae";
    public int sexType = 0;
    public string roomName = "Sc_t1";
    [Space(10)]
    public int MaxNumToJoinRoom = 20; //최대 접속자 수 :20명
    public bool IsIntoRoom = true;//룸에 입장하는 체크
    private PlayerInfo playerInfo;
    //public GameObject userPrefab;
    //버전 입력
    private readonly string version = "1.0f";
    ////사용자 아이디 입력

    public bool writeHashTable = false;

    private void Awake()
    {
        playerInfo = GetComponent<PlayerInfo>();
        if (IsOffLineMode)
        {
#if ForAndroid 
            GameObject.Find("Player").transform.tag = "Player";
#endif
            StartCoroutine(Disconnect());
        }
        else
        {
            PhotonNetwork.OfflineMode = false;

            userId = UserInfo.userName;
            sexType = UserInfo.chType;
            // 같은 룸의 유저들에게 자동으로 씬을 로딩
            PhotonNetwork.AutomaticallySyncScene = true;
            // 같은 버전의 유저끼리 접속 허용
            PhotonNetwork.GameVersion = version;
            // 유저 아이디 할당
            PhotonNetwork.NickName = userId;//playerinfo에서 이름 가져옴
                                            // 포톤 서버와 통신 횟수 설정 . 초당 30회\
            Debug.Log(PhotonNetwork.SendRate);
            // 서버 접속
            PhotonNetwork.ConnectUsingSettings();

        }

    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to Master!");
        Debug.Log($"PhotonNetwork.inLobby = {PhotonNetwork.InLobby}");
        if (IsIntoRoom)//룸에 입장하는 체크
            PhotonNetwork.JoinLobby(); // 로비 입장
    }
    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.inLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); //랜덤 매치메이킹
        PhotonNetwork.JoinRoom(roomName);
    }


    //룸 입장 실패 시에 호출
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //if (playerInfo.ccuRoom >= MaxNumToJoinRoom)//현재 사용자가 최대 조인 수 보다 많은면 연결종료
        //{
        //    Debug.Log($"접속자수가 {MaxNumToJoinRoom}보다 많아 접속종료합니다.");
        //    Disconnect();
        //    playerInfo.UseNetWork = false;//네트워크 비활성화
        //    PhotonNetwork.OfflineMode = true;
        //}
        Debug.Log($"JoinRandomFailed {returnCode }:{message }");
        //룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = MaxNumToJoinRoom; //최대 접속자 수 :20명
        ro.IsOpen = true; // 룸의 오픈 여부
        ro.IsVisible = true;// 로비에 룸 목록에 노출 시킬지 여부

        //룸 생성

        PhotonNetwork.CreateRoom(roomName, ro);
    }
    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"룸 이름은 = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PlayerCount = {PhotonNetwork.CurrentRoom.PlayerCount}");

        // 룸에 접속한 사용자 정보 확인
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {

            Debug.Log($"현재접속자는 {player.Value.NickName}이고,접속순번은{player.Value.ActorNumber}이다");
        }
        playerInfo = GetComponent<PlayerInfo>();
        if (writeHashTable)
        {
            Propoties = new Hashtable() { { "chType", UserInfo.chType }, { "name", UserInfo.userName } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(Propoties);

        }
        playerInfo.thePlayer = PhotonNetwork.Instantiate("user_Photon", transform.position, transform.rotation, 0);
    }

    public void LeaveRoom()        // 방 떠나기.
    {
        // 방 떠나기.
        PhotonNetwork.LeaveRoom();
    }
    //public void Disconnect()
    //{
    //    PhotonNetwork.Disconnect();
    //}
    public IEnumerator Disconnect()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("포톤 연결 끊기");
        if (PhotonNetwork.IsConnected == true)
            PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.OfflineMode = true;
    }
}
