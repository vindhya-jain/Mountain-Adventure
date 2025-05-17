using UnityEngine;
using UnityEngine.AI;

public class playerMovement : MonoBehaviour
{   [SerializeField]
    public float rotationSpeed=360;
    [SerializeField]
    public float jumpSpeed=5;
    [SerializeField]
    private float jumpHorizontalSpeed;
    public float jumpButtonGracePeriod=0.2f;

    [SerializeField]
    private Transform cameraTransform;
    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
            inputMagnitude /=2;
        }
        animator.SetFloat("inputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
        
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)*movementDirection;
        movementDirection.Normalize();
        
        ySpeed += Physics.gravity.y*Time.deltaTime;

        if(characterController.isGrounded){
            lastGroundedTime = Time.time;
        }

        if(Input.GetButtonDown("Jump")){
            jumpButtonPressedTime = Time.time;
        }

        if(Time.time - lastGroundedTime <= jumpButtonGracePeriod){
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);
            
            if(Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod){
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else{
            characterController.stepOffset = 0;
            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if((isJumping && ySpeed < 0)|| ySpeed < -10){
                animator.SetBool("isFalling", true);
            }
        }
        
        if(movementDirection != Vector3.zero){
            animator.SetBool("isMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
        else{
            animator.SetBool("isMoving", false);
        }

        if(isGrounded == false){
            Vector3 velocity = movementDirection*inputMagnitude*jumpHorizontalSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity*Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        if(isGrounded){
            Vector3 velocity = animator.deltaPosition;
            velocity.y = ySpeed*Time.deltaTime;

            characterController.Move(velocity);
        }
        
    }
    private void OnApplicationFocus(bool focus)
    {
        if(focus){
            Cursor.lockState = CursorLockMode.Locked;
        }
        else{
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
