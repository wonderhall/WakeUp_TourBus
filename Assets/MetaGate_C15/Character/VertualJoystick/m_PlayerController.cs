using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class m_PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    //public bool isMove;//pInfo.animIsMoving로 대체

    //private Player playerInput;
    public MyDefaultInputActions playerInput;
    //public Animator anim;

    private PlayerInfo pInfo;//플레이어인포
    private void Awake()
    {
        //playerInput = new Player();
        playerInput = new MyDefaultInputActions(); 
        controller = GetComponent<CharacterController>();
        
        pInfo= GameObject.FindObjectOfType<PlayerInfo>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

    }
    private void OnDisable()
    {
        playerInput.Disable();
    }


    private void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
       Vector2 movementInput = playerInput.MobileMove.Move.ReadValue<Vector2>();

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;


        if (movementInput.x != 0 || movementInput.y != 0) pInfo.IsWalk = true; else pInfo.IsWalk = false;

        controller.Move(move * playerSpeed * Time.deltaTime);

        //anim.SetBool("IsMove", pInfo.IsWalk);

        if (playerInput.MobileMove.Jump.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }
}
