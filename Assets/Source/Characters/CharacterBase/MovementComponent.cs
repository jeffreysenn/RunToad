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

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpeedZController))]
public class MovementComponent : MonoBehaviour
{
    [Header("Movement")]
    public float gravity = 10;
    public float walkSpeedZ = 15;
    public float walkSpeedX = 2;
    public float jumpHeight = 1;
    public float airControlZ = 1;
    public float airControlX = .5f;
    public float slideTime = 1;
    public float slideControlZ = 1;
    public float slideControlX = .2f;
    public float switchWallSpeedX = 4;
    public float switchWallControlZ = 1;
    public float switchAccepetanceRange = 1;
    public float switchRotateSpeed = 90;
    public float switchRotateAcceptanceRange = .0001f;

    [Header("Auto Run")]
    public bool autoRun = false;

    [Header("Advance")]
    public float groundCheckExtraRadius = -.01f;
    public float groundCheckOvershoot = .01f;
    public int collisionBuffer = 5;

    // TODO replace with animation
    public Vector3 slideTransform = new Vector3(0, -.6f, 0);

    private Vector3 velocity = Vector3.zero;
    private bool shouldJump = false, shouldSlide = false;
    private float movementAxisZ, movementAxisX;
    private float shouldSwitchAxis, switchAxis;
    private bool shouldRotate = false;
    private RaycastHit WallToSwitch;
    private float slideTimer = 0;


    private MovementState state = MovementState.Jumping;

    protected void Update()
    {

        ComputeRequestMovement();

        Move();
    }

    private void ComputeRequestMovement()
    {
        switch (state)
        {
            case MovementState.Grounding:
                MoveForward(movementAxisZ);
                MoveRight(movementAxisX);
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
                else if (shouldSwitchAxis != 0)
                {
                    switchAxis = shouldSwitchAxis;
                    SwitchWall(switchAxis);
                    state = MovementState.Switching;
                }
                break;
            case MovementState.Jumping:
                ClearRequests();
                MoveForward(airControlZ * movementAxisZ);
                MoveRight(airControlX * movementAxisX);
                if (IsMovingOnGround()) { state = MovementState.Grounding; }
                break;
            case MovementState.Sliding:
                ClearRequests();
                slideTimer += Time.deltaTime;
                MoveForward(slideControlZ * movementAxisZ);
                MoveRight(slideControlX * movementAxisX);
                if (slideTimer > slideTime)
                {
                    StopSliding();
                    slideTimer = 0;
                    state = MovementState.Grounding;
                }
                break;
            case MovementState.Switching:
                ClearRequests();
                MoveForward(switchWallControlZ * movementAxisZ);
                if (!shouldRotate)
                {
                    if (CheckWall(out WallToSwitch))
                    {
                        if (Vector3.Distance(transform.position, WallToSwitch.point) < switchAccepetanceRange)
                        {
                            shouldRotate = true;
                        }
                    }
                }
                else
                {
                    velocity.x = 0;
                    Rotate();
                    if (Mathf.Abs(1 - Vector3.Dot(transform.up, WallToSwitch.normal)) < switchRotateAcceptanceRange)
                    {
                        transform.rotation = WallToSwitch.transform.rotation;
                        shouldRotate = false;
                        switchAxis = 0;
                        state = MovementState.Grounding;
                    }
                }
                break;
            default:
                break;
        }
    }


    public void RequestMoveForward(float axisValue) { if (!autoRun) { movementAxisZ = axisValue; } }

    public void RequestMoveForwardAuto(float axisValue) { if (autoRun) { movementAxisZ = axisValue; } }

    public void RequestMoveRight(float axisValue) { movementAxisX = axisValue; }

    public void RequestJump() { shouldJump = true; }

    public void RequestSlide() { shouldSlide = true; }

    public void RequestSwitchWall(float axisValue) { shouldSwitchAxis = axisValue; }

    private void ClearRequests()
    {
        shouldJump = false;
        shouldSlide = false;
        shouldSwitchAxis = 0;
    }


    private void MoveForward(float axisValue) { velocity.z = axisValue * walkSpeedZ; }

    private void MoveRight(float axisValue) { velocity.x = axisValue * walkSpeedX; }

    private void Jump() { velocity.y = Mathf.Sqrt(2 * gravity * jumpHeight); }

    private void Fall() { velocity.y -= gravity * Time.deltaTime; }

    private void StopFalling() { if (velocity.y < 0) { velocity.y = 0; } }

    private void Slide() { transform.localScale += slideTransform; }

    private void StopSliding() { transform.localScale -= slideTransform; }

    private void SwitchWall(float axisValue)
    {
        velocity.x = axisValue * switchWallSpeedX;
    }

    private bool CheckWall(out RaycastHit wall)
    {
        // TODO Add layer mask
        return Physics.Raycast(transform.position, switchAxis * transform.right, out wall, Mathf.Infinity);

    }

    private void Rotate()
    {
        transform.RotateAround(transform.TransformPoint(Vector3.down * GetCapsuleCylinderHalfHeight()), switchAxis*transform.forward, Time.deltaTime * switchRotateSpeed);

    }

    private void Move()
    {
        transform.Translate(velocity * Time.deltaTime);

        ApplyPhysicsY();

        HandleCollision();
    }

    private void ApplyPhysicsY()
    {
        RaycastHit outGround;
        if (!IsMovingOnGround(out outGround))
        {
            Fall();
        }
        else
        {
            StopFalling();

            ClampToGround(outGround);
        }
    }

    private void HandleCollision()
    {
        Collider[] outOverlappingColliders = new Collider[collisionBuffer + 1];
        int numOverlappingColliders = Physics.OverlapCapsuleNonAlloc(transform.position + transform.up * GetCapsuleCylinderHalfHeight(), transform.position - transform.up * GetCapsuleCylinderHalfHeight(), GetCapsuleRadius(), outOverlappingColliders);
        for(int i = 0; i < numOverlappingColliders; i++)
        {
            if(outOverlappingColliders[i] == GetComponent<CapsuleCollider>()) { continue; }
            Vector3 outDirection = Vector3.zero;
            float outDistance = 0;
            if (Physics.ComputePenetration(GetComponent<CapsuleCollider>(), transform.position, transform.rotation, outOverlappingColliders[i], outOverlappingColliders[i].transform.position, outOverlappingColliders[i].transform.rotation, out outDirection, out outDistance))
            {
                Vector3 penetration = outDirection * outDistance;
                transform.position += penetration;
            }
        }
    }

    private bool IsMovingOnGround(out RaycastHit ground)
    {
        Ray ray = new Ray(transform.position + transform.up * (GetCapsuleHalfHeight() - GetGroundCheckRadius()), -transform.up);
        return Physics.SphereCast(ray, GetGroundCheckRadius(), out ground, 2 * (GetCapsuleHalfHeight() - GetGroundCheckRadius()) + GetGroundCheckOvershoot());
    }

    private bool IsMovingOnGround()
    {
        Ray ray = new Ray(transform.position + transform.up * (GetCapsuleHalfHeight() - GetGroundCheckRadius()), -transform.up);
        return Physics.SphereCast(ray, GetGroundCheckRadius(), 2 * (GetCapsuleHalfHeight() - GetGroundCheckRadius()) + GetGroundCheckOvershoot());
    }

    private void ClampToGround(RaycastHit ground) { transform.position = ground.point + ground.normal * GetGroundCheckRadius() + transform.up * (GetCapsuleHalfHeight() - GetGroundCheckRadius()); } 
    private float GetCapsuleCylinderHalfHeight() { return (GetComponent<CapsuleCollider>().height / 2 - GetComponent<CapsuleCollider>().radius) * transform.localScale.y; }
    private float GetCapsuleRadius() { return GetComponent<CapsuleCollider>().radius * Mathf.Max(transform.localScale.x, transform.localScale.z); }
    private float GetCapsuleHalfHeight() { return GetComponent<CapsuleCollider>().height / 2 * transform.localScale.y; }
    private float GetGroundCheckRadius() { return (GetComponent<CapsuleCollider>().radius - groundCheckExtraRadius) * Mathf.Max(transform.localScale.x, transform.localScale.z); }
    private float GetGroundCheckOvershoot() { return groundCheckOvershoot * transform.localScale.y; }

}
