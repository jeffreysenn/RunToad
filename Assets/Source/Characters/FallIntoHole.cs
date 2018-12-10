using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallIntoHole : MonoBehaviour
{

	public void Fall(Transform holeTransform)
    {
        transform.SetPositionAndRotation(holeTransform.position, holeTransform.rotation);
        GetComponent<MovementComponent>().RequestFreezeAllMovement();
    }
}
