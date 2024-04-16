using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemTuotorial : MonoBehaviour
{
    #region sample 1
    //public GameObject obj;
    //MyDefaultInputActions playerController;
    //public bool useOldInputSystem;

    //public float timeBetweenSpawns = 0.25f;
    //float currentTime;

    //private void Awake()
    //{
    //    playerController = new MyDefaultInputActions();
    //}
    //// Start is called before the first frame update
    //private void OnEnable()
    //{
    //    playerController.Enable();
    //}
    //private void OnDisable()
    //{
    //    playerController.Disable();
    //}
    //private void Update()
    //{
    //    if (useOldInputSystem) OldInputSystem();
    //    else NewInputSystem();
    //}

    //void OldInputSystem()
    //{
    //    currentTime += Time.deltaTime;
    //    if (Input.GetKey(KeyCode.Space))
    //    { 
    //    print("Old input system key is down");
    //        if(currentTime > timeBetweenSpawns)
    //        {
    //            Destroy(Instantiate(obj, transform.position, Quaternion.identity), 30f);
    //            currentTime = 0;
    //        }
    //    }

    //}
    //void NewInputSystem()
    //{
    //    bool isSpaceKeyHeld = playerController.Player.SpaceKey.ReadValue<float>()>0.1f;
    //    if (isSpaceKeyHeld)
    //    {
    //        print("new input system key is down");
    //        if (currentTime > timeBetweenSpawns)
    //        {
    //            Destroy(Instantiate(obj, transform.position, Quaternion.identity), 30f);
    //            currentTime = 0;
    //        }
    //    }
    //}
    #endregion
    #region sample 2
    public InputActionReference ToggleReference = null;
    public GameObject toggleObject = null;
    private void OnEnable()
    {
        ToggleReference.action.performed += ToggleObj;
        ToggleReference.action.Enable();
    }

    private void OnDisable()
    {
        ToggleReference.action.performed -= ToggleObj;
        ToggleReference.action.Disable();

    }
    private void ToggleObj(InputAction.CallbackContext callbackContext)
    {
        print("callbackContext");
        print("≈‰±€");
        bool isAcitive = !toggleObject.activeSelf;
        toggleObject.SetActive(isAcitive);
    }
    #endregion
}