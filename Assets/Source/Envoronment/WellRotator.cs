using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellRotator : MonoBehaviour {
    public float rotationAngle = 45;
    public float rotationTime = .5f;

    private float axisValue = 0;
    private bool shouldRotate = false;
    private float elapsedTime = 0;

    public void RequestRotate(float value)
    {
        axisValue = value;
        shouldRotate = true;
    }

    protected void Update()
    {
        float rotZOri = transform.rotation.z;
        if(elapsedTime < rotationTime && shouldRotate)
        {
            float rotZ = Mathf.Lerp(rotZOri, rotZOri + axisValue * rotationAngle, rotationTime);
            transform.Rotate(0, 0, rotZ);
            elapsedTime += Time.deltaTime;
        }

    }

}
