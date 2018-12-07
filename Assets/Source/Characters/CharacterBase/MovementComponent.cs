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
public class MovementComponent : MonoBehaviour
{
    public float gravity = 10;
    public float walkSpeedZ = 5;
    public float walkSpeedX = 2;
    public float jumpHeight = 1;
    public float airControlZ = 1;
    public float airControlX = .5f;
    public float slideTime = 1;
    public float slideControlZ = 1;
    public float slideControlX = .2f;
    public float switchAccepetanceRange = 2;

    public int collisionBuffer = 5;
    // TODO replace with animation
    public Vector3 slideTransform = new Vector3(0, -.8f, 0);

    private Vector3 velocity = Vector3.zero;
    private bool shouldJump = false, shouldSlide = false;
    private float movementAxisZ, movementAxisX, switchAxis;
    private float slideTimer = 0;


    private MovementState state = MovementState.Jumping;

    protected void Update()
    {

        ComputeVelocity();

        Move();
    }

    private void ComputeVelocity()
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
                else if (switchAxis != 0)
                {
                    state = MovementState.Switching;
                }
                break;
            case MovementState.Jumping:
                RejectActionRequests();
                MoveForward(airControlZ * movementAxisZ);
                MoveRight(airControlX * movementAxisX);
                if (IsMovingOnGround()) { state = MovementState.Grounding; }
                break;
            case MovementState.Sliding:
                RejectActionRequests();
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
                RejectActionRequests();
                SwitchWall(switchAxis);
                break;
            default:
                break;
        }
    }


    public void RequestMoveForward(float axisValue) { movementAxisZ = axisValue; }

    public void RequestMoveRight(float axisValue) { movementAxisX = axisValue; }

    public void RequestJump() { shouldJump = true; }

    public void RequestSlide() { shouldSlide = true; }

    public void RequestSwitchWall(float axisValue) { switchAxis = axisValue; }

    private void RejectActionRequests()
    {
        shouldJump = false;
        shouldSlide = false;
        //switchAxis = 0;
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
                Debug.Log(penetration);
                transform.position += penetration;
            }
        }
    }

    private bool IsMovingOnGround(out RaycastHit ground)
    {
        RaycastHit outHit;

        if (Physics.SphereCast(transform.position + transform.up * GetCapsuleCylinderHalfHeight(), GetCapsuleRadius(), -transform.up, out outHit, GetCapsuleCylinderHalfHeight() * 2))
        {
            ground = outHit;
            return true;
        }
        else
        {
            ground = new RaycastHit();
            return false;
        }
    }

    private bool IsMovingOnGround()
    {
        RaycastHit outHit;

        if (Physics.SphereCast(transform.position + transform.up * GetCapsuleCylinderHalfHeight(), GetCapsuleRadius(), -transform.up, out outHit, GetCapsuleCylinderHalfHeight() * 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClampToGround(RaycastHit ground) { transform.position = ground.point + ground.normal * GetCapsuleRadius() + transform.up * GetCapsuleCylinderHalfHeight(); } 
    private float GetCapsuleCylinderHalfHeight() { return (GetComponent<CapsuleCollider>().height / 2 - GetComponent<CapsuleCollider>().radius) * transform.localScale.y; }
    private float GetCapsuleRadius() { return GetComponent<CapsuleCollider>().radius * Mathf.Max(transform.localScale.x, transform.localScale.z); }

}
