using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerController : MonoBehaviour     
{
    public Camera MyCamera;
    public float Speed = 2f;
    public float SprintSpeed = 5f;
    public float RotationSpeed = 15;
    public float AnimationBlendSpeed = 2f;
    public Animator MyAnimator;
    public CharacterController MyController;
    float mDesiredAnimationSpeed = 0f;
    float mDesiredRotation = 0f;
    bool mSprinting = false;
    private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;


    void Start()
    {
        MyController = GetComponent<CharacterController>();
        MyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(x, 0, z).normalized;

        Vector3 rotatedMovement = Quaternion.Euler(0, MyCamera.transform.rotation.eulerAngles.y,0) * movement;
  
        MyController.Move((rotatedMovement *(mSprinting ? SprintSpeed : Speed)) * Time.deltaTime);

        if (rotatedMovement.magnitude > 0)
        {
            mDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;
            mDesiredAnimationSpeed = mSprinting ? 1 : .5f;
        }
        else
        {
            mDesiredAnimationSpeed = 0;
        }

        MyAnimator.SetFloat("Speed", Mathf.Lerp(MyAnimator.GetFloat("Speed"), mDesiredAnimationSpeed, AnimationBlendSpeed * Time.deltaTime));

        if(rotatedMovement.magnitude > 0)
        {
           mDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;
        }
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, mDesiredRotation, 0);

        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, RotationSpeed * Time.deltaTime ); 
    }
    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        MyController.Move(velocity * Time.deltaTime);

        if(isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        

    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }
    
}
