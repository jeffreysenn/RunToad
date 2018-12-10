using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class LevelDestroyer : MonoBehaviour {

    public float destroyDistanceBehind = 0f;
    GameObject mainCamera;

	void Start ()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

    }

    void Update ()
    {
        transform.position = mainCamera.transform.position - transform.forward * ((GetComponent<BoxCollider>().size.z/2) + destroyDistanceBehind);
	}

    private void OnTriggerEnter(Collider other)
    {

        Vector3 selfCentre = transform.position;
        float selfHalfLength = transform.localScale.z * GetComponent<BoxCollider>().size.z / 2;
        Vector3 otherCentre = Vector3.zero;
        float otherHalfLength = 0;

        if (other.gameObject.GetComponent<BoxCollider>() != null)
        {
            otherCentre = transform.TransformPoint(other.gameObject.GetComponent<BoxCollider>().center);
            otherHalfLength = other.gameObject.GetComponent<BoxCollider>().size.z * other.gameObject.transform.localScale.z;
        }
        else if (other.gameObject.GetComponent<Renderer>() != null)
        {
            otherCentre = other.gameObject.GetComponent<Renderer>().bounds.center;
            otherHalfLength = other.gameObject.GetComponent<Renderer>().bounds.extents.magnitude;
        }
        else
        {
            return;
        }

        if (GetFrontCentrePosition(selfCentre, selfHalfLength).z > GetFrontCentrePosition(otherCentre, otherHalfLength).z) { Destroy(other.gameObject); }
    }

    private Vector3 GetFrontCentrePosition(Vector3 centrePosition, float halfLength)
    {
        return centrePosition + transform.forward * halfLength;
    }

}
