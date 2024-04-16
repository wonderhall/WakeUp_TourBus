using UnityEngine;


public class CameraLook : MonoBehaviour
{

    [SerializeField]
    private float lookSpeed = 1;
 //   private CinemachineFreeLook cinemachine;

    private MyDefaultInputActions playerInput;
    public Transform playerBody;
    private PlayerInfo pf;
    float xRotation = 0f;
    private void Awake()
    {
        pf = GameObject.FindObjectOfType<PlayerInfo>(); 
        playerInput = new MyDefaultInputActions();
        
     //   cinemachine = GetComponent<CinemachineFreeLook>();

    }
    private void OnEnable()
    {
        playerInput.Enable();

    }
    private void OnDisable()
    {
        playerInput.Disable();
    }



    void Update()
    {
        Vector2 delta = playerInput.MobileMove.Look.ReadValue<Vector2>();

        float  mouseX = delta.x *  lookSpeed * Time.deltaTime;
        float mouseY = delta.y *  lookSpeed * Time.deltaTime;



        if (delta != Vector2.zero)
        {
            playerBody.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            pf.IsRotate = true;
        }
        else pf.IsRotate = false;
       

    }
}
