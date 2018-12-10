using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehaviour : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FallIntoHole>() != null)
        {
            other.GetComponent<FallIntoHole>().Fall(transform);
        }
    }
}
