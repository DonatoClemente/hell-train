using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HellTrain.PlayerSystems
{
    public class Player_Animation_Handler : MonoBehaviour
    {   
        [SerializeField] InputController inputController;
        [SerializeField] AnimatorController animatorControl;
        [SerializeField] private HealthBar health;
        [SerializeField] private PlayerInput controls;

        [Header("Other Body Parts")]
        [SerializeField] AnimatorController playerArm;

        //  audio for sfx
        [Header("Audio FX")]
        [SerializeField] AudioClip walkSFX;
        [SerializeField] AudioClip pushpullSFX;
        [SerializeField] AudioClip revolverSFX;
        [SerializeField] AudioClip hurtSFX;
        [SerializeField] AudioClip deathSFX;
        

        [Header("Animation Timings (seconds)")]
        [SerializeField] private float hurtTime;
        private bool isAnimationLocked = false;

        public UnityEvent Hurt;
        public UnityEvent Death;

        // Start is called before the first frame update
        void Start()
        {
            if(inputController == null)
                inputController = gameObject.GetComponentInParent<InputController>();

            animatorControl = gameObject.GetComponent<AnimatorController>();
            health = GetComponentInChildren<HealthBar>();
            controls = inputController.playerControls;
        }
        // Update is called once per frame
        void Update()
        {
            // If game is paused or the play is hurt, do not follow through this script's update
            if(inputController.gameStateManager.isPaused || isAnimationLocked)
                return;

            if(health.WasHit())
                HurtPlayer();
            
        }
    public void HurtPlayer()
    {   
        isAnimationLocked = true;
        animatorControl.CrossFade("Player Hurt");
        StartCoroutine(animationLockOut(hurtTime));
        StartCoroutine(controlsLockOut(hurtTime));
        AudioManager.Instance.PlaySoundFX(hurtSFX);
        Hurt.Invoke();
    } 

    public void DeadPlayer()
    {
        isAnimationLocked = true;
        controls.Player.Disable();
        animatorControl.CrossFade("Player Death");
        Death.Invoke();
    }

    public void FireRevolver()
    {
        playerArm.CrossFade("Fire");
        AudioManager.Instance.PlaySoundFX(revolverSFX);
    }


    private IEnumerator animationLockOut(float time)
    {
        yield return new WaitForSeconds(time);
        isAnimationLocked = false;
    }

    private IEnumerator controlsLockOut(float time)
    {
        controls.Player.Disable();
        yield return new WaitForSeconds(time);
        controls.Player.Enable();
    }    
    }

}

