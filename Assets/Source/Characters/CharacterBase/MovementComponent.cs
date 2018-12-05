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
    public float gravity = 10;
    public float walkSpeed = 5;
    public float sideWalkSpeed = 2;
    public float jumpHeight = 1;
    public float airControlVertical = 1;
    public float airControlHorizontal = .5f;
    public float slideTime = 1;
    public float slideControlVertical = 1;
    public float slideControlHorizontal = .2f;
    public float switchAccepetanceRange = 2;

    // TODO replace with animation
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
                else if (switchAxisValue != 0)
                {
                    state = MovementState.Switching;
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
                ClearShould();
                SwitchWall(switchAxisValue);
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

    private void SwitchWall(float axisValue)
    {
        RaycastHit outHit;
        // TODO Add layer mask
        Physics.Raycast(transform.position/* - new Vector3(0, characterController.height/2, 0)*/, axisValue * transform.right, out outHit, 20);
        Debug.Log(outHit.point);
        MoveRight(axisValue);
        if(Vector3.Distance(transform.position, outHit.point) < switchAccepetanceRange)
        {
            transform.rotation = outHit.transform.rotation;
        }
    }
}
