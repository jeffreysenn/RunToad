using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    Grounding,
    Jumping,
    Switching,
};

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    public float gravity = 10;
    public float walkSpeed = 5;
    public float sideWalkSpeed = 2;
    public float jumpHeight = 1;
    public float airControlVertical = 1;
    public float airControlHorizontal = .5f;


    private Vector3 velocity = Vector3.zero;
    private bool shouldJump;
    private float verticleAxisValue, honrizontalAxisValue, switchAxisValue;


    private CharacterController characterController;
    private MovementState state = MovementState.Jumping;

    protected void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    protected void Update()
    {
        characterController.Move(transform.TransformDirection(velocity) * Time.deltaTime);
        if (!characterController.isGrounded) { Fall(); } else { StopFalling(); }

        switch (state)
        {
            case MovementState.Grounding:
                MoveForward(verticleAxisValue);
                MoveRight(honrizontalAxisValue);
                if (shouldJump)
                {
                    Jump();
                    state = MovementState.Jumping;
                }
                break;
            case MovementState.Jumping:
                shouldJump = false;
                MoveForward(airControlVertical * verticleAxisValue);
                MoveRight(airControlHorizontal * honrizontalAxisValue);
                if (characterController.isGrounded) { state = MovementState.Grounding; }
                break;
            case MovementState.Switching:

                break;
            default:
                break;
        }
    }

    public void RequestMoveForward(float axisValue) { verticleAxisValue = axisValue; }

    public void RequestMoveRight(float axisValue) { honrizontalAxisValue = axisValue; }

    public void RequestJump() { shouldJump = true; }

    public void RequestSwitchWall(float axisValue) { switchAxisValue = axisValue; }

    private void MoveForward(float axisValue) { velocity.z = axisValue * walkSpeed; }

    private void MoveRight(float axisValue) { velocity.x = axisValue * sideWalkSpeed; }

    private void Jump() { velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight); }

    private void Fall() { velocity.y -= gravity * Time.deltaTime; }

    private void StopFalling() { velocity.y = 0; }

    private void SwitchLane()
    {

    }
}
