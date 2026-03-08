using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float jumpForce;
    private float inputHorizontal;
    private float inputVertical;
    private Rigidbody2D rb;
    private bool facingRight = true;
    [Header("Jump")]
    public int extraJumpsValue;
    public float jumpTime;
    private int extraJumps;
    private float jumpTimeCounter;
    private bool isJumping;
    [Header("Ground")]
    private bool isGrounded;
    private bool wasGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;
    [Header("Ladder")]
    public float distance;
    public LayerMask whatIsLadder;
    private bool isClimbing;
    RaycastHit2D hitInfo;
    [Header("Effect")]
    public GameObject dustEffect;
    private bool spawnDust;
    public Animator camAnimator;

    public Animator playerAnimator;

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        inputHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(inputHorizontal * speed, rb.linearVelocity.y);
        if (Mathf.Abs(inputHorizontal) > 0 && isGrounded)
        {
            playerAnimator.SetBool("PlayerRun", true);
        }
        else {
            playerAnimator.SetBool("PlayerRun", false);
                }

        if (facingRight == false && inputHorizontal > 0)
        {
            Flip();
            //playerAnimator.SetBool("PlayerRun", true);
        }
        else if (facingRight == true && inputHorizontal < 0)
        {
            Flip();
            //playerAnimator.SetBool("PlayerRun", false);
        }
    }
    void Update()
    {
        if (isGrounded == true)
        {
            if (wasGrounded == false)
            {
                playerAnimator.SetTrigger("PlayerLand");
            }
            if (spawnDust == true)
            {
                if(camAnimator)
                {
                    camAnimator.SetTrigger("Shake");
                }
                if(spawnDust)
                { 
                    Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
                    spawnDust = false;
                }
              
            }
        }
        else
        {
            spawnDust = true;
        }

        if (isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            if (rb.linearVelocity.y == 0)
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
            playerAnimator.SetTrigger("PlayerJump");
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            Instantiate(dustEffect, groundCheck.position, Quaternion.identity);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);
        if (hitInfo.collider != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
        }

        if (isClimbing == true && hitInfo.collider != null)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputVertical * speed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 5;
        }
        PlayerInput();
        wasGrounded = isGrounded;
    }
    
    void PlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.E)) playerAnimator.SetTrigger("PlayerSpawn");
        if (Input.GetKeyDown(KeyCode.Q)) playerAnimator.SetTrigger("PlayerHurt");
        if (Input.GetKeyDown(KeyCode.R)) playerAnimator.SetTrigger("PlayerVictory");

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}


