using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private GameObject playerObj;
    private Vector3 offset;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    void Start ()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj == null) { return; }
        offset = transform.position - playerObj.transform.position;
	}
	
	void LateUpdate ()
    {
        transform.LookAt(playerObj.transform);
        //transform.rotation = playerObj.transform.rotation;
        Vector3 targetPosition = playerObj.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
