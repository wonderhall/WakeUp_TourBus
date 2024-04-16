using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public static string userName = "noName";
    public static int chType = 0;
    private static UserInfo _Instance;
    public static UserInfo Instancet
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<UserInfo>();
                _Instance.name = _Instance.GetType().ToString();
            }
            return _Instance;
        }
    }
    //private void Awake()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}
}
