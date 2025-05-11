using UnityEngine;

public class CowboyMovement : MonoBehaviour
{
    private float horizontalMovement;
    private float speed = 3f;
    private float jumpingPower = 4f;
    private bool isFacingRight = true;

    private int jumpCount = 0;
    private int maxJumps = 1;
    private bool isGrounded = false;

    [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpCount++;
            Debug.Log("Jump count: " + jumpCount);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) 
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}