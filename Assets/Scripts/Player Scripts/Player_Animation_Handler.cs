using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HellTrain.PlayerSystems
{
    public class Player_Animation_Handler : MonoBehaviour
    {   [SerializeField] PlayerController playerController;
        [SerializeField] AnimatorController animatorControl;
        [SerializeField] private HealthBar health;
        [SerializeField] private PlayerInput controls;

        //  audio for sfx
    /*   [Header("Audio")]
        [SerializeField] AudioSource walkSFX;
        [SerializeField] AudioSource pushpullSFX;
        [SerializeField] AudioSource hurtSFX;
        [SerializeField] AudioSource deathSFX;
        private AudioSource currentsound;*/

        [Header("Animation Timings (seconds)")]
        [SerializeField] private float hurtTime;
        private bool isAnimationLocked = false;

        public UnityEvent Hurt;
        public UnityEvent Death;

        // Start is called before the first frame update
        void Start()
        {
            if(playerController == null)
                playerController = gameObject.GetComponentInParent<PlayerController>();

            animatorControl = gameObject.GetComponent<AnimatorController>();
            health = GetComponentInChildren<HealthBar>();
            controls = playerController.playerControls;
        }
        // Update is called once per frame
        void Update()
        {
                    // If game is paused or the play is hurt, do not follow through this script's update
            //if(playerController.gameStateManager.isPaused || isAnimationLocked)
            //return;
        }
    }

}

