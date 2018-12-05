using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    private MovementComponent movementComp;

    void Start()
    {
        GameObject playerCharacterObj = GameObject.FindGameObjectWithTag("Player");
        if(playerCharacterObj == null)
        {
            Debug.Log("Player object not found");
            return;
        }

        movementComp = playerCharacterObj.GetComponent<MovementComponent>();
        if(movementComp == null)
        {
            Debug.Log("Player movement component not found");
            return;
        }
    }

    void Update()
    {
        movementComp.RequestMoveForward(Input.GetAxis("Vertical"));
        movementComp.RequestMoveRight(Input.GetAxis("Horizontal"));
        if (Input.GetButton("Jump")) { movementComp.RequestJump(); }
        if (Input.GetButton("Slide")) { movementComp.RequestSlide(); }
        movementComp.RequestSwitchWall(Input.GetAxis("SwitchWall"));


    }

}
