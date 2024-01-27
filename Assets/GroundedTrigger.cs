using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Helltrain.PlayerSystems
{
    public class GroundedTrigger : MonoBehaviour
    {

        [SerializeField] CharacterController2D characterController2D;

        void Start()
        {
            characterController2D = GetComponentInParent<CharacterController2D>();
        }


        void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.gameObject.layer == 6)
            { 
                characterController2D.m_Grounded = true;
                characterController2D.justLanded = true;
                characterController2D.isFalling = false;
                characterController2D.isWallClinging = false;
                characterController2D.recentlyWallJumped = false;
                characterController2D.isFallTransitioning = false;
            }
        }
        
        void OnTriggerStay2D(Collider2D collider)
        {
            if(collider.gameObject.layer == 6)
            { 
                characterController2D.m_Grounded = true;
                characterController2D.justLanded = false;
                characterController2D.isFalling = false;
                characterController2D.isWallClinging = false;
                characterController2D.isFallTransitioning = false;
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if(collider.gameObject.layer == 6)
                characterController2D.m_Grounded = false;
        }
    }
}
