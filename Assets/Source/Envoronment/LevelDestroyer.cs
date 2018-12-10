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

    private void OnTriggerStay(Collider other)
    {

        Vector3 selfCentre = transform.position;
        float selfLength = transform.localScale.z * GetComponent<BoxCollider>().size.z / 2;

        Vector3 otherCentre = other.gameObject.GetComponent<Renderer>().bounds.center;
        float otherLength = other.gameObject.GetComponent<Renderer>().bounds.extents.magnitude ;

        if (GetFrontCentrePosition(selfCentre, selfLength).z > GetFrontCentrePosition(otherCentre, otherLength).z) { Destroy(other.gameObject); }
    }

    private Vector3 GetFrontCentrePosition(Vector3 centrePosition, float length)
    {
        return centrePosition + transform.forward * length;
    }

}
