using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbPlayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animPlayer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float castWidth;


    private float dirX;
    private float animTime = 0.5f;
    private bool isAttacking;
    private bool isJumping = false;
    private bool isGrounded = true;

    private string currentAnimName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        // 1 if player move forward, 0 if player don't move and -1 if player move back
        dirX = Input.GetAxisRaw("Horizontal");

        if(isAttacking){
            rbPlayer.velocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }
            
            /* Player moves */
            if (Mathf.Abs(dirX) > 0.1f)
            {
                Movement();

            }
            else if (isGrounded)
            {
                ChangeAnim("idle");
                rbPlayer.velocity = Vector2.zero;
            }

            /* Player jump */
            if (Input.GetKey(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
            else if (!isGrounded && rbPlayer.velocity.y < 0)
            {
                ChangeAnim("fall");
            }

            /* Player slash */
            if (Input.GetMouseButton(0) && isGrounded)
            {
                Slash();
            }

            /* Player throw weapon */
            if (Input.GetKey(KeyCode.E) && isGrounded)
            {
                Throw();
            }
        }

        // Check player is falling
        if (!isGrounded && rbPlayer.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }
    }

    //When player moves
    private void Movement()
    {
        ChangeAnim("run");
        rbPlayer.velocity = new Vector2(dirX * Time.fixedDeltaTime * moveSpeed, rbPlayer.velocity.y);

        //rotation player if dirX  > 0 return 0, if dirX <= 0 return 180
        transform.rotation = Quaternion.Euler(new Vector3(0, dirX > 0 ? 0 : 180, 0));
    }

    //When player jumps
    private void Jump()
    {
        ChangeAnim("jump");
        isJumping = true;
        rbPlayer.AddForce(jumpForce * Vector2.up);
    }
    //When player attacks
    private void Slash()
    {
        ChangeAnim("attack");
        isAttacking = true;
        Invoke(nameof(ResetAttack), animTime);
    }

    //When player throws
    private void Throw()
    {
        ChangeAnim("throw");
        isAttacking = true;
        Invoke(nameof(ResetAttack), animTime);
    }

    //Restet animation player attack
    private void ResetAttack()
    {
        isAttacking = false;
        ChangeAnim("idle");
    }

    //Check player on the ground
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * castWidth, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, castWidth, groundLayer);
        // Debug.Log(hit.collider.name);
        return hit.collider != null;
    }

    //Changes animator player
    private void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            animPlayer.ResetTrigger(animName);
            currentAnimName = animName;
            animPlayer.SetTrigger(currentAnimName);
        }
    }
}
