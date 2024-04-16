using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAinmationAction;
    public Animator handanimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float triggerValue =  pinchAnimationAction.action.ReadValue<float>();
        handanimator.SetFloat("Trigger", triggerValue);
        float gripValue = gripAinmationAction.action.ReadValue<float>();
        handanimator.SetFloat("Grip", gripValue);
    }
}
