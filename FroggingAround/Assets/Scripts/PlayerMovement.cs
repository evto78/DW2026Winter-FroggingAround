using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb; bool noGravity;

    public float sensitivity;
    public GameObject cam;
    float yaw = 0.0f;
    float pitch = 0.0f;

    public float baseMoveSpeed;
    float baseSprintMoveSpeed;
    public float baseJumpForce;
    int baseNumberOfJumps;

    public float friction;

    float moveSpeed;
    float sprintMoveSpeed;
    float jumpForce;
    float airStrafeSpeed;
    int numberOfJumps;
    int jumpsLeft;
    float gravityModifier;

    public bool onGround;
    bool isSprinting;

    Vector3 inputDir;

    void Start()
    {
        jumpsLeft = numberOfJumps;
        rb = GetComponent<Rigidbody>();

        baseSprintMoveSpeed = baseMoveSpeed * 1.6f;
        baseNumberOfJumps = 1;

        moveSpeed = baseMoveSpeed;
        sprintMoveSpeed = baseSprintMoveSpeed;
        jumpForce = baseJumpForce;
        airStrafeSpeed = moveSpeed * 0.25f;
        numberOfJumps = baseNumberOfJumps;
        gravityModifier = 1f;

        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        onGround = GroundCheck();
        if (Cursor.lockState == CursorLockMode.Locked) { CameraMove(); }
        GetInputs();
    }
    private void FixedUpdate()
    {
        if (!onGround && !noGravity)
        {
            rb.AddForce(-Vector3.up * 10 * Time.deltaTime * gravityModifier);
        }
        if (Cursor.lockState == CursorLockMode.Locked) { Move(); } else { Friction(); }
    }
    bool GroundCheck()
    {
        if (Physics.BoxCast(new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), transform.localScale * 0.5f, -Vector3.up, out RaycastHit hit, transform.rotation, 1f))
        {
            if (hit.transform.gameObject.CompareTag("Ground") || hit.transform.gameObject.CompareTag("Untagged"))
            {
                jumpsLeft = numberOfJumps;
                return true;
            }
        }

        return false;
    }
    void CameraMove()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            //get mouse input
            yaw += sensitivity * Input.GetAxis("Mouse X");
            pitch -= sensitivity * Input.GetAxis("Mouse Y");

            //limit cam angle
            pitch = Mathf.Clamp(pitch, -85.0f, 85.0f);

            //set cam angle
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yaw, transform.eulerAngles.z);
            cam.transform.eulerAngles = new Vector3(pitch, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
    void effects()
    {

    }
    void GetInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isSprinting) { isSprinting = false; } else { isSprinting = true; }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { inputDir += Vector3.forward; }
        if (Input.GetKey(KeyCode.S)) { inputDir -= Vector3.forward; }
        if (Input.GetKey(KeyCode.A)) { inputDir -= Vector3.right; }
        if (Input.GetKey(KeyCode.D)) { inputDir += Vector3.right; }
        inputDir = Vector3.Normalize(inputDir);
    }
    void Move()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (onGround)
        {
            rb.AddRelativeForce(inputDir * airStrafeSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (isSprinting)
        {
            rb.AddRelativeForce(inputDir * sprintMoveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            rb.AddRelativeForce(inputDir * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        //Limit Velocity realative to speed
        if (noGravity) { return; }
        Vector3 limitedVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (isSprinting)
        {
            if (limitedVelocity.magnitude >= sprintMoveSpeed / 100f)
            {
                limitedVelocity = Vector3.Normalize(limitedVelocity);
                limitedVelocity = limitedVelocity * sprintMoveSpeed / 100f;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
        else
        {
            if (limitedVelocity.magnitude >= moveSpeed / 100f)
            {
                limitedVelocity = Vector3.Normalize(limitedVelocity);
                limitedVelocity = limitedVelocity * moveSpeed / 100f;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        Friction();
    }
    void Jump()
    {
        if (jumpsLeft > 0 && onGround)
        {
            jumpsLeft -= 1;
            rb.AddForce((transform.up+(transform.forward/2f)) * jumpForce, ForceMode.Force);
        }
    }
    void Friction()
    {
        if (onGround)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.useGravity = true;
            flatVel = flatVel / (friction * (1 + Time.deltaTime));
            rb.velocity = new Vector3(flatVel.x, rb.velocity.y / (1 + Time.deltaTime), flatVel.z);
        }
        else if (!noGravity) { rb.useGravity = true; }
    }
}
