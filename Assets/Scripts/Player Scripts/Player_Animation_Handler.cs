using System.Collections;
using System.Collections.Generic;
using Helltrain;
using UnityEngine;
using UnityEngine.Events;

namespace HellTrain.PlayerSystems
{
    public class Player_Animation_Handler : MonoBehaviour
    {   
        [SerializeField] InputController inputController;
        [SerializeField] AnimatorController animatorControl;
        [SerializeField] CharacterController2D characterController2D;
        [SerializeField] private HealthBar health;
        [SerializeField] private PlayerInput controls;

        [Header("Other Body Parts")]
        [SerializeField] AnimatorController playerArm;

        //  audio for sfx
        [Header("Audio FX")]
        [SerializeField] AudioClip walkSFX;
        [SerializeField] AudioClip runSFX;
        [SerializeField] AudioClip jumpSFX;
        [SerializeField] AudioClip landSFX;
        [SerializeField] AudioClip slideSFX;
        [SerializeField] AudioClip revolverSFX;
        [SerializeField] AudioClip hurtSFX;
        [SerializeField] AudioClip deathSFX;
        

        [Header("Animation Timings (seconds)")]
        [SerializeField] private float hurtTime;
        private bool isAnimationLocked = false;
        private bool justFiredGun = false;
        private bool canPlayFootstepSFX = true;
        private bool playingLandingAnim = false;

        public UnityEvent Hurt;
        public UnityEvent Death;

        // Start is called before the first frame update
        void Start()
        {
            if(inputController == null)
                inputController = GetComponentInParent<InputController>();

            characterController2D = GetComponentInParent<CharacterController2D>();
            animatorControl = GetComponent<AnimatorController>();
            health = GetComponentInChildren<HealthBar>();
            controls = inputController.playerControls;
        }
        // Update is called once per frame
        void Update()
        {
            // If game is paused or the play is hurt, do not follow through this script's update
            if(inputController.gameStateManager.isPaused)
                return;

            if(health.CurrentHP() <= 0)
                DeadPlayer();
            else if(health.WasHit())
                HurtPlayer();

            if(isAnimationLocked)
                return;



            if(characterController2D.justLanded && characterController2D.m_Grounded)
                {animatorControl.CrossFade("Landing"); StartCoroutine(JustLanded());        StartCoroutine(footstepSoundFX(landSFX));}   
            else if(characterController2D.isIdle && characterController2D.m_Grounded && !playingLandingAnim && !justFiredGun)
                {animatorControl.CrossFade("Idle");     return;}

            else if(!characterController2D.isIdle && characterController2D.m_Grounded && !playingLandingAnim)
            {                       
                if(characterController2D.isWalking && !justFiredGun)
                    {animatorControl.CrossFade("Walk");                 StartCoroutine(footstepSoundFX(walkSFX));}
                else if(characterController2D.isRunning && !justFiredGun)
                    {animatorControl.CrossFade("Run");                  StartCoroutine(footstepSoundFX(runSFX));}
                else if(characterController2D.isRunning && !justFiredGun)
                    animatorControl.CrossFade("Run n Aim");                
            }
            else if(characterController2D.isWallClinging)
            {  
                if(characterController2D.isGoingUp)
                    {animatorControl.CrossFade("Wall Jump");}
                else if(characterController2D.justWallLanded)
                    {animatorControl.CrossFade("Wall Land");            StartCoroutine(footstepSoundFX(landSFX));}
                else if(characterController2D.isWallClinging && characterController2D.isIdle)
                    animatorControl.CrossFade("Wall No Velocity");
                else if(characterController2D.isWallClinging && characterController2D.isFallTransitioning)
                    {animatorControl.CrossFade("Wall Slide Medium");    StartCoroutine(footstepSoundFX(slideSFX));}
                else if(characterController2D.isWallClinging && characterController2D.isFalling)
                    {animatorControl.CrossFade("Wall Slide Fast");      StartCoroutine(footstepSoundFX(slideSFX));}                
            }

            else if(!characterController2D.m_Grounded && !characterController2D.recentlyWallJumped)
            {
                if(characterController2D.isGoingUp)
                    animatorControl.CrossFade("Jump Up");
                else if(characterController2D.isFallTransitioning)
                    animatorControl.CrossFade("Falling Start");
                else if(characterController2D.isFalling)
                    animatorControl.CrossFade("Fall Down");                
            }
            
        }


    public void JumpAnimation()
    {
        if(characterController2D.justWallJumped)
            animatorControl.CrossFade("Wall Jump");
        else 
            animatorControl.CrossFade("Jump Up");

        AudioManager.Instance.PlaySoundFX(jumpSFX);
    }

    public void HurtPlayer()
    {   
        isAnimationLocked = true;
        animatorControl.CrossFade("Hurt");
        StartCoroutine(animationLockOut(hurtTime));
        StartCoroutine(controlsLockOut(hurtTime));
        AudioManager.Instance.PlaySoundFX(hurtSFX);
        Hurt.Invoke();
    } 

    public void DeadPlayer()
    {
        isAnimationLocked = true;
        controls.Player.Disable();
        animatorControl.CrossFade("Death");
        Death.Invoke();
    }

    public void FireRevolver()
    {   if(characterController2D.isWallClinging)
            return;
        justFiredGun = true;
        if(characterController2D.isIdle)
            animatorControl.CrossFade("Gun Shoot");
        else
            animatorControl.CrossFade("Run n Gun");
        //playerArm.CrossFade("Gun Shoot");
        AudioManager.Instance.PlaySoundFX(revolverSFX);
        StartCoroutine(JustFired());
    }

    private IEnumerator JustFired()
    {
        yield return new WaitForSeconds(.6f);
        justFiredGun = false;
    }

    private IEnumerator JustLanded()
    {
        playingLandingAnim = true;
        yield return new WaitForSeconds(0.3f);
        playingLandingAnim = false;
    }

    private IEnumerator animationLockOut(float time)
    {
        isAnimationLocked = true;
        yield return new WaitForSeconds(time);
        isAnimationLocked = false;
    }

    private IEnumerator controlsLockOut(float time)
    {
        controls.Player.Disable();
        yield return new WaitForSeconds(time);
        controls.Player.Enable();
    }    


    // For audioclips that will get called in the Update() method
    private IEnumerator footstepSoundFX(AudioClip clip)
    {
        if(canPlayFootstepSFX)
        {   
            AudioManager.Instance.PlayFootStepFX(clip);
            canPlayFootstepSFX = false;
        }
        else
        {   yield break;    }

        yield return new WaitForSeconds(clip.length * 0.75f);
        canPlayFootstepSFX = true;
    }
    }

}

