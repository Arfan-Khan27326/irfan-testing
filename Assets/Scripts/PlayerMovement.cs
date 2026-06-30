  using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private LayerMask wallLayer;  
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private void Awake()
    { 
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    { 
       // flip player when moving left or right
        horizontalInput = Input.GetAxis("Horizontal");
      
        if (horizontalInput > 0.01f) 
            transform.localScale = Vector3.one;
        
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        
         // set animator parameters
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());
        
        wallclimb();
    }

    private void wallclimb()
    {
        if (wallJumpCooldown > 0.2f)
        {

              body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
        if (onWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = 5;
            
            if (Input.GetKey(KeyCode.Space))
                jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
    private void jump()

    { 
        if (IsGrounded())
        {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
       }
       else if (onWall() && !IsGrounded())
        {

            if (horizontalInput == 0)
            {
           body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 8, jumpPower);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
           body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5, jumpPower);
           wallJumpCooldown = 0; 
        }
    }
    
    private bool IsGrounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundlayer);
          return raycast.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycast.collider != null;
    }
    public bool canAttack()
    {
        return horizontalInput == 0 && IsGrounded() && !onWall();
    }
}
