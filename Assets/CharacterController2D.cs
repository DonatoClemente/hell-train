using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor.Callbacks;

namespace Helltrain
{
    public class CharacterController2D : MonoBehaviour
    {
        [Tooltip("The modifer of your speed for walking, running, and crouching")]
        [SerializeField] private float targetHorizontalVelocity = 10f;	
        [Tooltip("The velocity of the character's jump")]
        [SerializeField] private float m_JumpVelocity = 15f;							// Amount of force added when the player jumps.
        [Tooltip("Modifier to movement speed when crouching [1 = 100%]")]
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
        [Tooltip("How much to smooth out the movement")]        
        [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
        [Tooltip("This determines if horizontal movement is normalized or not")]
        [SerializeField] private bool normalizeMovement = false;	
        [Tooltip("Whether or not a character can steer while jumping")]
        [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
        [Tooltip("A mask determining what is ground to the character")]
        [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
        [Tooltip("A position marking where to check if the character is grounded.")]
        [SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
        [Tooltip("A position marking where to check for ceilings")]
        [SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
        [Tooltip("A collider that will be disabled when crouching")]
        [SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching


        const float k_GroundedRadius = .2f;                         // Radius of the overlap circle to determine if grounded
        const float k_CeilingRadius = .2f;                          // Radius of the overlap circle to determine if the player can stand up
        const float gravity = 9.8f;
        public bool m_Grounded;                                     // Whether or not the player is grounded.
        [SerializeField] bool canDoubleJump = false;                // Enables double jumping
        [SerializeField] bool CoyoteTime = false;                   // Enables Coyote Time Mechanic
        [SerializeField] bool m_FacingRight = true;                           // For determining which way the player is currently facing.
        private float trueDoubleJumpTimer = 0f;    
        public float coyotetimer = .25f;
        private float m_startingCoyoteTime;
        public float wallDirection = 0;
        private Rigidbody2D m_Rigidbody2D;
        private Vector3 m_Velocity = Vector3.zero;

        [Header("Events")]
        [Space]

        public UnityEvent OnLandEvent;

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent OnCrouchEvent;
        private bool m_wasCrouching = false;
        public bool isIdle = false;
        public bool isWalking = false;
        public bool isRunning = false;
        public bool isGoingUp = false;
        public bool isFallTransitioning = false;
        public bool isFalling = false;
        public bool justLanded = false;
        public bool justWallLanded = false;
        public bool justWallJumped = false;
        public bool recentlyWallJumped = false;
        public bool isWallClinging = false;
        

        public bool enableFlipping = true;
        

        // Start is called before the first frame update
        void Awake()
        {
            if(!m_Rigidbody2D)
            m_Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
            m_startingCoyoteTime = coyotetimer;

            if (OnLandEvent == null)
                OnLandEvent = new UnityEvent();

            if (OnCrouchEvent == null)
                OnCrouchEvent = new BoolEvent();
        }

        private void Update()
        {
            if(trueDoubleJumpTimer > 0)
                trueDoubleJumpTimer -= Time.deltaTime;		

            if(coyotetimer > 0)
                coyotetimer -= Time.deltaTime;
            
            if(isGoingUp && m_Rigidbody2D.velocity.y <= 0.5f)
                isGoingUp = false;

            if(Mathf.Abs(m_Rigidbody2D.velocity.x) <= 0.01f && Mathf.Abs(m_Rigidbody2D.velocity.y) <= 0.01f)
            {   
                isIdle = true;
                isRunning = isFalling = false;
            }
            else
                isIdle = false;
            
            if(m_Grounded)
            {
                if(m_Rigidbody2D.velocity.x != 0)
                    isWalking = true;
                if(Mathf.Abs(m_Rigidbody2D.velocity.x) > 3f)
                    {isRunning = true;      isWalking = false;}
            }

            if(Mathf.Abs(m_Rigidbody2D.velocity.y) <= 0.5f && !m_Grounded)
                    isFallTransitioning = true;
            else if(m_Rigidbody2D.velocity.y < -0.5f)
                    {isFallTransitioning = false; isFalling = true;}

        }   

        private void FixedUpdate()
        {
            // Check if the rigidbody is grounded
            // Apply gravity only if the rigidbody is not grounded
            if(!m_Grounded)
                ApplyGravity();
            else
            {
                //m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                coyotetimer = m_startingCoyoteTime;
            }

            

            Mathf.Clamp(Mathf.Abs(m_Rigidbody2D.velocity.y), 0f, 50f);
            Mathf.Clamp(Mathf.Abs(m_Rigidbody2D.velocity.x), 0f, 50f);
        }     

        public void Jump()
        {
            // If the Character should jump... 
            // Seperate logic for Coyote Time 
            if(CoyoteTime)
            {	
                    // If you aren't jumping already, and you either are grounded or have some coyote time left	
                    if (( m_Grounded || coyotetimer > 0) && !isGoingUp && !isWallClinging)
                    {	
                            // These are commented out, but useful for testing
                            // Debug.Log("isGrounded set to: " + m_Grounded );
                            // Debug.Log("CoyoteTimer is at: " + coyotetimer + " and isGoingUp is " + isGoingUp);
                        
                        // Add a vertical force to the player.
                        coyotetimer = 0;
                        isGoingUp = true;
                        trueDoubleJumpTimer = 0.2f;
                        m_Grounded = false;
                       
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpVelocity);
                        
                    }
                    else if(!isWallClinging && canDoubleJump && trueDoubleJumpTimer <= 0)
                    {                 
                        trueDoubleJumpTimer = 0.55f;
                        canDoubleJump = false;
                       
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpVelocity* 0.75f);
                    }
                    else if(isWallClinging && !justWallJumped)
                    {   
                        isGoingUp = true;
                        trueDoubleJumpTimer = 0.2f;
                        justWallJumped = true;
                        Flip();
                        StartCoroutine(JustWallJumped());
                        
                        m_Rigidbody2D.velocity = new Vector2(-wallDirection *  m_JumpVelocity*.6f, m_JumpVelocity);
                    }
            }
            else
            {       // Check if they are grounded and aren't jumping yet
                    if (m_Grounded && !isGoingUp)
                    {
                        // Add a vertical force.
                        isGoingUp = true;
                        
                        trueDoubleJumpTimer = 0.2f;
                        canDoubleJump = false;
                        m_Grounded = false;
                        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpVelocity));
                    }
                    else if(!m_Grounded && canDoubleJump && trueDoubleJumpTimer <= 0 )
                    {  
                        trueDoubleJumpTimer = 0.55f;
                        canDoubleJump = false;
                        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpVelocity * 0.75f));
                    }
            }
        }

        IEnumerator JustWallJumped()
        {
            recentlyWallJumped = true;
            yield return new WaitForSeconds(0.125f);
            justWallJumped = false;
            yield return new WaitForSeconds(0.5f);
            recentlyWallJumped = false;
        }

        public void Move(Vector2 movement, bool crouch)
        {  
            float move;
            if(normalizeMovement)
                move = movement.x * targetHorizontalVelocity;
            else
                move = System.MathF.Sign(movement.x) * targetHorizontalVelocity;
      
            if(justWallJumped)
                return;
            
            // If crouching, check to see if the character can stand up
            if (!crouch)
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                   // crouch = true;
                }
              
                    //crouch = false;
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // If crouching
                if(crouch)
                {
                    if (!m_wasCrouching)
                    {
                        m_wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }
 
                    // Reduce the speed by the crouchSpeed multiplier
                    move *= m_CrouchSpeed;

                    // Disable one of the colliders when crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = false;
                } 
                else
                {
                    // Enable the collider when not crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = true;

                    if (m_wasCrouching)
                    {
                        m_wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }
                
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                if(move != 0)
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                else
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing*2);
                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
        }   
    
    
        private void ApplyGravity()
        {
            // Gravity * The exponential constant * fixedDeltaTime 
            Vector2 gravityForce = new Vector2(0f, -gravity * 2.718f * Time.fixedDeltaTime);
            m_Rigidbody2D.velocity +=  gravityForce;
        }


        public void Flip()
        {
            if(!enableFlipping)
                return;

            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Rotate 180 degrees to flip
            transform.root.Rotate(0,180,0);
        }


        
        void OnCollisionStay2D(Collision2D other)
        {
            // If the contact point is on our side, and we aren't ground, nor falling we are wall clinging
            if(other.gameObject.layer == 6)
                if(!m_Grounded && !isGoingUp && m_Rigidbody2D.velocity.y != 0)
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y * .95f);
        }







        /// <summary>
        ///                     Depricated code below. No longer necesary
        ///                     
        /// </summary>
        /// <returns></returns>


        private bool IsGrounded()
        {
            bool wasGrounded = m_Grounded;
            justLanded = false;	

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers 
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {   
                if (colliders[i].gameObject != gameObject && (m_Rigidbody2D.velocity.y == 0))
                {	
                    m_Grounded = true;	

                    if (!wasGrounded){
                        justLanded = true;		
                        trueDoubleJumpTimer = 0;
                        OnLandEvent.Invoke();
                    }		
                    break;	
                }
                else
                {   
                    m_Grounded = false;
                }      
            }
            
            return m_Grounded;
        }
    }
}
