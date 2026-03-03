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
    public float baseSprintMoveSpeed;
    public float baseJumpForce;
    public int baseNumberOfJumps;

    public float friction;

    public float moveSpeed;
    public float sprintMoveSpeed;
    public float jumpForce;
    public float airStrafeSpeed;
    public int numberOfJumps;
    int jumpsLeft;
    public float gravityModifier;

    bool onGround;
    public bool isSprinting;
    public float timeSinceGrounded;

    Vector3 inputDir;

    // Start is called before the first frame update
    void Start()
    {
        jumpsLeft = numberOfJumps;
        rb = GetComponent<Rigidbody>();

        baseMoveSpeed = 600f;
        baseSprintMoveSpeed = baseMoveSpeed * 1.6f;
        baseJumpForce = 2000f;
        baseNumberOfJumps = 1;

        moveSpeed = baseMoveSpeed;
        sprintMoveSpeed = baseSprintMoveSpeed;
        jumpForce = baseJumpForce;
        airStrafeSpeed = moveSpeed * 0.5f;
        numberOfJumps = baseNumberOfJumps;
        gravityModifier = 1f;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceGrounded += Time.deltaTime;

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
        if (Physics.BoxCast(new Vector3(transform.position.x, transform.position.y - 0f, transform.position.z), transform.localScale * 0.5f, -Vector3.up, out RaycastHit hit, transform.rotation, 1f))
        {
            if (hit.transform.gameObject.CompareTag("Ground") || hit.transform.gameObject.CompareTag("Untagged"))
            {
                jumpsLeft = numberOfJumps;
                timeSinceGrounded = 0f;
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
        if (jumpsLeft > 0)
        {
            jumpsLeft -= 1;
            rb.AddForce(transform.up * jumpForce, ForceMode.Force);
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
