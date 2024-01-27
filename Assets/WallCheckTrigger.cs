using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Helltrain.PlayerSystems
{
    public class WallCheckTrigger : MonoBehaviour
    {

        [SerializeField] CharacterController2D characterController2D;

        void Start()
        {
            characterController2D = GetComponentInParent<CharacterController2D>();
        }


        void OnTriggerEnter2D(Collider2D collider)

        {   Vector2 closepoint = collider.ClosestPoint(transform.position);
            float angleFromCheck = Vector2.Angle(transform.position, closepoint);

            if(collider.gameObject.layer == 6 && angleFromCheck < 40)
            {   
                characterController2D.isWallClinging = true;
                characterController2D.recentlyWallJumped = false;
                characterController2D.wallDirection = Mathf.Sign(collider.gameObject.transform.position.x - transform.position.x);
                if(!characterController2D.m_Grounded)
                    StartCoroutine(JustWallLanded());
            }
        }
        
        void OnTriggerStay2D(Collider2D collider)
        {
            if(collider.gameObject.layer == 6)
            {
                
                characterController2D.isWallClinging = true;
                characterController2D.wallDirection = Mathf.Sign(collider.gameObject.transform.position.x - transform.position.x);
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if(collider.gameObject.layer == 6)
            {
                characterController2D.isWallClinging = false;
                characterController2D.justWallLanded = false;
                characterController2D.justWallJumped = false;
                characterController2D.wallDirection = 0;
            }
        }

        IEnumerator JustWallLanded()
        {characterController2D.justWallLanded = true;
            yield return new WaitForSeconds(.4f);
            characterController2D.justWallLanded = false;
        }
    }
}
