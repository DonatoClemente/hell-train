using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helltrain
{
    public class WallCheckTrigger : MonoBehaviour
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
                characterController2D.isWallClinging = true;
                characterController2D.wallDirection = Mathf.Sign(collider.gameObject.transform.position.x - transform.position.x);
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
                characterController2D.wallDirection = 0;
            }
        }
    }
}
