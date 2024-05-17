using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GokalpCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravityMultiplier = 2f;
    public LayerMask groundLayer;
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Kendi yerçekimi uygulayacaðýmýz için kapatýyoruz
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(moveHorizontal, 0f, moveVertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    void ApplyGravity()
    {
        Vector3 gravity = Vector3.down * gravityMultiplier * Physics.gravity.magnitude;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
