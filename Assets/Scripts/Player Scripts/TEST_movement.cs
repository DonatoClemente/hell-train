using UnityEngine;

public class TEST_movement : MonoBehaviour
{

    //
    //      CAUTION:  THIS IS JUST A CHATGPT SCRIPT TO GET SOMETHING MOVING, I AM LAZY RN. pls delete later.
    //                                                                                        - Trey
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        // Move left or right
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new Vector2(horizontalInput, 0);
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}
