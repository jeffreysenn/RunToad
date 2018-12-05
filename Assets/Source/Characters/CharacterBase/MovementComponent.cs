using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    Grounding,
    Jumping,
    Sliding,
    Switching,
};

[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    public const float gravity = 10;
    public const float walkSpeed = 5;
    public const float sideWalkSpeed = 2;
    public const float jumpHeight = 1;
    public const float airControlVertical = 1;
    public const float airControlHorizontal = .5f;
    public const float slideTime = 1;
    public const float slideControlVertical = 1;
    public const float slideControlHorizontal = .2f;

    // TODO replace with real animation
    public Vector3 slideTransform = new Vector3(0, -.8f, 0);

    private Vector3 velocity = Vector3.zero;
    private bool shouldJump = false, shouldSlide = false;
    private float verticleAxisValue, horizontalAxisValue, switchAxisValue;
    private float slideTimer = 0;

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
                MoveRight(horizontalAxisValue);
                if (shouldJump)
                {
                    Jump();
                    state = MovementState.Jumping;
                }
                else if (shouldSlide)
                {
                    Slide();
                    state = MovementState.Sliding;
                }
                break;
            case MovementState.Jumping:
                ClearShould();
                MoveForward(airControlVertical * verticleAxisValue);
                MoveRight(airControlHorizontal * horizontalAxisValue);
                if (characterController.isGrounded) { state = MovementState.Grounding; }
                break;
            case MovementState.Sliding:
                ClearShould();
                slideTimer += Time.deltaTime;
                MoveForward(slideControlVertical * verticleAxisValue);
                MoveRight(slideControlHorizontal * horizontalAxisValue);
                if(slideTimer > slideTime)
                {
                    StopSliding();
                    slideTimer = 0;
                    state = MovementState.Grounding;
                }
                break;
            case MovementState.Switching:

                break;
            default:
                break;
        }
    }

    public void RequestMoveForward(float axisValue) { verticleAxisValue = axisValue; }

    public void RequestMoveRight(float axisValue) { horizontalAxisValue = axisValue; }

    public void RequestJump() { shouldJump = true; }

    public void RequestSlide() { shouldSlide = true; }

    public void RequestSwitchWall(float axisValue) { switchAxisValue = axisValue; }

    public void ClearShould()
    {
        shouldJump = false;
        shouldSlide = false;
    }

    private void MoveForward(float axisValue) { velocity.z = axisValue * walkSpeed; }

    private void MoveRight(float axisValue) { velocity.x = axisValue * sideWalkSpeed; }

    private void Jump() { velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight); }

    private void Fall() { velocity.y -= gravity * Time.deltaTime; }

    private void StopFalling() { velocity.y = 0; }

    private void Slide() { transform.localScale += slideTransform; }

    private void StopSliding() { transform.localScale -= slideTransform; }

    private void SwitchLane()
    {

    }
}
