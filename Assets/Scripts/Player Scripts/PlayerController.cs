using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HellTrain
{
    public class PlayerController : MonoBehaviour
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
    

        
        public PlayerInput playerControls;

        // All player inputs must have their own local variable
        private InputAction move;
        private InputAction fire;        
        private InputAction jump;
        private InputAction ult;
        private InputAction pause;


        // This will hold the direction we want to move later
        Vector3 movedirection = Vector3.zero;

        // Awake is called before Start()
        // only use this to find things inside this gameobject
        // Start() is for finding things in other gameobjects
        void Awake()
        {
            playerControls = new PlayerInput();

            move    = playerControls.FindAction("Move");
            fire    = playerControls.FindAction("Fire");
            jump    = playerControls.FindAction("Jump"); 
            ult     = playerControls.FindAction("ULT");
            pause   = playerControls.FindAction("Pause");
        }

        // Everytime the 'InputAction' is .performed, call method
        void OnEnable()
        {
            move.performed      += OnMove;
            fire.performed      += OnFire;
            jump.performed      += OnJump;
            ult.performed       += OnUlt;
            pause.performed     += OnPause;

            playerControls.Enable();
        }
        // Turn off controller basically, do the reverse of above
        void OnDisable()
        {
            move.performed      -= OnMove;
            fire.performed      -= OnFire;
            jump.performed      -= OnJump;
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
        movedirection = context.ReadValue<Vector2>();
    }
    /******************************************************************************************************************
            Fire:

                -Does Something
    */
    private void OnFire(InputAction.CallbackContext context)
    {

    }
    /******************************************************************************************************************
            Jump:

                -Go up in the Air when you press the jump button
    */
    private void OnJump(InputAction.CallbackContext context)
    {

    }
    /******************************************************************************************************************
            Ultimate:

                -Does Something
    */
    private void OnUlt(InputAction.CallbackContext context)
    {

    }
    /******************************************************************************************************************
            Pause Game:

                -Opens up the Pause menu 
    */
    private void OnPause(InputAction.CallbackContext context)
    {

    }


    }
}
