using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using Helltrain;
using System;
using TMPro;
using UnityEngine.UIElements;
using TreeEditor;
using System.Collections;

namespace HellTrain.PlayerSystems
{
    public class InputController : MonoBehaviour
    {
        
        /// <summary>
        ///
        ///     This script is ONLY for taking in inputs from Controllers/Keyboard. If you want to create a new control or
        ///     ability that uses some button press, you must first go to the PlayerInput asset under "/Asset/Input Controls".
        ///     If you want to learn more about Unity's Input System watch "https://youtu.be/m5WsmlEOFiA?si=eBXWSUFplcAVH1U_".
        ///     
        ///     Setting up a new input in this script follows this format:
        ///                 - Create a private local InputAction, all lowercase for uniformity.      
        ///                 - In Awake() set that variable to what ever you named it in the PlayerInput asset
        ///                 - Follow the OnEnble / OnDisable format. The += means to "Subscribe" using Unity's Event System
        ///                 - What follows the += symbol the name of the new method you'll make to house the logic
        ///                 - Make the method private with the return type void. Names of methods must start with "On" for uniformity
        ///                 - Put (InputAction.CallbackContext context) as the only parameter of this method. FINISHED. 
        /// 
        /// </summary>  
    

        [SerializeField] public GameStateManager gameStateManager;
        [SerializeField] Player_Animation_Handler playerAnimation;
        [SerializeField] CharacterController2D characterController2D;
        public PlayerInput playerControls;

        [SerializeField] GameObject GhostGrapple;

        // All player inputs must have their own local variable
        private InputAction move;
        private InputAction fire;        
        private InputAction jump;
        private InputAction grapple;
        private InputAction ult;
        private InputAction pause;


        // This will hold the direction we want to move later
        Vector3 movedirection = Vector3.zero;
        private GameObject ghosthand;
        // This will let us know if the player is trying to crouch
        private bool isCrouching = false;
        private bool isGrappling = false;
        private bool canFire = true;
        // Awake is called before Start()
        // only use this to find things inside this gameobject
        // Start() is for finding things in other gameobjects
        void Awake()
        {
            gameStateManager = FindAnyObjectByType<GameStateManager>();
            characterController2D = GetComponent<CharacterController2D>();
            playerControls = new PlayerInput();

            move    = playerControls.FindAction("Move");
            fire    = playerControls.FindAction("Fire");
            jump    = playerControls.FindAction("Jump"); 
            grapple = playerControls.FindAction("Grapple");
            ult     = playerControls.FindAction("ULT");
            pause   = playerControls.FindAction("Pause");
        }

        // Everytime the 'InputAction' is .performed, call method
        void OnEnable()
        {
            move.performed      += OnMove;
            move.canceled       += CancelMove;
            fire.performed      += OnFire;
            jump.performed      += OnJump;
            grapple.performed   += OnGhostGrapple;
            grapple.canceled    += CancelGhostGrapple;
            ult.performed       += OnUlt;
            pause.performed     += OnPause;

            playerControls.Enable();
        }
        // Turn off controller basically, do the reverse of above
        void OnDisable()
        {
            move.performed      -= OnMove;
            move.canceled       -= CancelMove;
            fire.performed      -= OnFire;
            jump.performed      -= OnJump;
            grapple.performed   -= OnGhostGrapple;
            grapple.canceled    -= CancelGhostGrapple;
            ult.performed       -= OnUlt;
            pause.performed     -= OnPause;

            playerControls.Disable();
        }     

    /******************************************************************************************************************
            Movement:

                -Reads the values given from controller input and put them into a variable
    */    
    private void OnMove(InputAction.CallbackContext context)
    {
        if(gameStateManager.isPaused)
            return;

        movedirection = context.ReadValue<Vector2>();
        movedirection.Normalize();
      
        isCrouching = movedirection.y < 0;
    }
    private void CancelMove(InputAction.CallbackContext context)
    {
        movedirection = new Vector3();
        isCrouching = false;
    }
    /******************************************************************************************************************
            Fire:

                -Does Something
    */
    private void OnFire(InputAction.CallbackContext context)
    {
        if(gameStateManager.isPaused || !canFire)
            return;

        playerAnimation.FireRevolver();
    }
    /******************************************************************************************************************
            Jump:

                -Go up in the Air when you press the jump button
    */
    private void OnJump(InputAction.CallbackContext context)
    {
        if(gameStateManager.isPaused)
            return;

        characterController2D.Jump();
    }
    /******************************************************************************************************************
            Ghost Grapple:

                -Pivot from a point in the air
                -Spawns a cool ghosty hand
    */
    private void OnGhostGrapple(InputAction.CallbackContext context)
    {
        return;
        if(gameStateManager.isPaused || isGrappling)
            return;

        if(characterController2D.m_Grounded || characterController2D.isWallClinging || characterController2D.isGoingUp)
            return;
 
        ghosthand = Instantiate(GhostGrapple);
        ghosthand.transform.position = transform.position + new Vector3(0,1);
        ghosthand.transform.eulerAngles = transform.eulerAngles;
        characterController2D.enableFlipping = false;
        GetComponentInChildren<RelativeJoint2D>().enabled = true;
        isGrappling = true;
        StartCoroutine(AttachGrapple());
        GetComponentInChildren<RelativeJoint2D>().connectedBody = ghosthand.transform.Find("Ghost Hand/Ghost Grapple/Player Grapple Point").GetComponent<Rigidbody2D>();
    }

        IEnumerator AttachGrapple()
        { 
            float i = .4f;
            while(i > 0)
            {
                if(ghosthand)
                    ghosthand.transform.position = transform.position + new Vector3(0,1);
                i -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if(ghosthand)
            {ghosthand.transform.position = transform.position+ new Vector3(0,1);
             ghosthand.GetComponentInChildren<Animator>().enabled = false;}
        }

    private void CancelGhostGrapple(InputAction.CallbackContext context)
    {
        if(!isGrappling || gameStateManager.isPaused) 
            return;

        isGrappling = false;
        characterController2D.enableFlipping = true;
        GetComponentInChildren<RelativeJoint2D>().enabled = false;
        Destroy(ghosthand);
    }

        /******************************************************************************************************************
       Ultimate:

           -Does Something
*/
        private void OnUlt(InputAction.CallbackContext context)
    {
        Debug.Log("Ult button pressed");
    }
    /******************************************************************************************************************
            Pause Game:

                -Opens up the Pause menu 
    */
    private void OnPause(InputAction.CallbackContext context)
    {
        if(!gameStateManager.isPaused)
        {
            gameStateManager.PauseGame();
            gameStateManager.isPaused = true;
        }   
        else if(gameStateManager.isPaused)
        {
            gameStateManager.UnpauseGame();
            gameStateManager.isPaused = false;
        }
    }

    void FixedUpdate()
    {
        characterController2D.Move(movedirection, isCrouching);
    }

    IEnumerator GunTimer()
    {
        canFire = false;
        yield return new WaitForSeconds(.6f);
        canFire = true;
    }

    }
}
